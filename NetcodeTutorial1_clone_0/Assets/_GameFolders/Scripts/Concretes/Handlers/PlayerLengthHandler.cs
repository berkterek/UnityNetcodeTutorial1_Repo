using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLengthHandler : NetworkBehaviour
{
    [SerializeField] GameObject _tailPrefab;
    [SerializeField] Transform _transform;
    
    //NetworkVariable means all clients sync this value and this value changes only by server side
    public NetworkVariable<ushort> Length = new(1);

    List<GameObject> _tails;
    Transform _lastTail;
    Collider2D _collider2D;

    public static event System.Action<ushort> OnLenghtValueChanged;

    void OnValidate()
    {
        if (_transform == null)
        {
            _transform = transform;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        this.enabled = IsOwner;
        _tails = new List<GameObject>();
        _lastTail = _transform;
        _collider2D = GetComponent<Collider2D>();

        //This Length value changed only call server thats why adding this if check "!IsServer" means server already call this only clients attached this callback
        if (!IsServer) Length.OnValueChanged += HandleOnLengthValueChanged;
    }

    //when value changed all clients invoke this method / if client is host this means he is server
    void HandleOnLengthValueChanged(ushort previousValue, ushort newValue)
    {
        Debug.Log(nameof(HandleOnLengthValueChanged) + " Callback");
        ChangeLengthValue(); // Client
    }

    //this will be called by server
    public void AddLength()
    {
        Length.Value += 1;
        ChangeLengthValue(); // Server
    }

    private void ChangeLengthValue()
    {
        InstantiateTail();
        if (!IsOwner) return;
        OnLenghtValueChanged?.Invoke(Length.Value);
        ClientSoundManager.Instance.PlayNomAudioClip();
    }

    private void InstantiateTail()
    {
        GameObject tailObject = Instantiate(_tailPrefab, _transform.position, Quaternion.identity);
        tailObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = -Length.Value;
        if (tailObject.TryGetComponent(out TailController tailController))
        {
            tailController.NetworkOwner = _transform;
            tailController.FollowTransform = _lastTail;
            _lastTail = tailController.transform;
            Physics2D.GetIgnoreCollision(tailController.GetComponent<Collider2D>(), _collider2D);
        }
        
        _tails.Add(tailObject);
    }
}
