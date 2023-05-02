using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _transform;
    [SerializeField] PlayerLengthHandler _playerLengthHandler;

    Camera _mainCamera;
    Vector3 _mouseInput = Vector3.zero;
    bool _canCollide = true;
    readonly ulong[] _targetClientsArray = new ulong[1];

    public static event System.Action OnGameOveredEvent;

    private void Initialize()
    {
        this.enabled = IsOwner;
        _mainCamera = Camera.main;
        _transform = transform;
    }

    void OnValidate()
    {
        if (_playerLengthHandler == null)
        {
            _playerLengthHandler = GetComponent<PlayerLengthHandler>();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    void Update()
    {
        if (!Application.isFocused) return;

        MovePlayerServer();
    }

    void MovePlayerServer()
    {
        _mouseInput = (Vector2)Input.mousePosition;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
        mouseWorldCoordinates.z = 0f;
        MovePlayerServerRpc(mouseWorldCoordinates);
    }

    [ServerRpc]
    void MovePlayerServerRpc(Vector3 mouseWorldCoordinates)
    {
        transform.position = Vector3.MoveTowards
        (
            _transform.position,
            mouseWorldCoordinates,
            Time.deltaTime * _moveSpeed
        );

        if (mouseWorldCoordinates != _transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - _transform.position;
            targetDirection.z = 0f;
            _transform.up = targetDirection;
        }
    }

    //Client authority movement
    void MovePlayerClient()
    {
        _mouseInput = (Vector2)Input.mousePosition;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
        mouseWorldCoordinates.z = 0f;
        transform.position = Vector3.MoveTowards
        (
            _transform.position,
            mouseWorldCoordinates,
            Time.deltaTime * _moveSpeed
        );

        if (mouseWorldCoordinates != _transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - _transform.position;
            targetDirection.z = 0f;
            _transform.up = targetDirection;
        }
    }

    //ServerRpc means this code triggered on client send to server only server run this code
    //RequireOwnerShip means this if equal true only owner can send this method to server and server updated only owner client see result if its equal false its global and owner send this to server and everybody see result
    [ServerRpc(RequireOwnership = false)]
    private void DetermineCollisionWinnerServerRpc(PlayerData player1, PlayerData player2)
    {
        if (player1.Length > player2.Length)
        {
            WinInformationServerRpc(player1.Id, player2.Id);
        }
        else
        {
            WinInformationServerRpc(player2.Id, player1.Id);
        }
    }

    [ServerRpc]
    private void WinInformationServerRpc(ulong winner, ulong loser)
    {
        _targetClientsArray[0] = winner;
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = _targetClientsArray
            }
        };
        AtePlayerClientRpc(clientRpcParams);

        _targetClientsArray[0] = loser;
        clientRpcParams.Send.TargetClientIds = _targetClientsArray;
        GameOverClientRpc(clientRpcParams);
    }

    //ClientRpc means this code triggered on Server send to client
    [ClientRpc]
    private void AtePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("You ate a player");
    }

    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("You lose");
        OnGameOveredEvent?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Player Collision");
        if (!other.gameObject.CompareTag("Player")) return;
        if (!IsOwner) return;
        if (!_canCollide) return;
        StartCoroutine(CollisionCheckAsync());

        if (other.collider.TryGetComponent(out PlayerLengthHandler playerLengthHandler))
        {
            var player1 = new PlayerData
            {
                Id = OwnerClientId,
                Length = _playerLengthHandler.Length.Value
            };

            var player2 = new PlayerData
            {
                Id = playerLengthHandler.OwnerClientId,
                Length = playerLengthHandler.Length.Value
            };
            DetermineCollisionWinnerServerRpc(player1, player2);
        }
        else if (other.collider.TryGetComponent(out TailController tailController))
        {
            var id = tailController.NetworkOwner.GetComponent<PlayerController>().OwnerClientId;

            if (id == OwnerClientId) return;
            
            Debug.Log("Tail Collision");
            WinInformationServerRpc(id, OwnerClientId);
        }
    }

    private IEnumerator CollisionCheckAsync()
    {
        _canCollide = false;
        yield return new WaitForSeconds(0.5f);
        _canCollide = true;
    }

    struct PlayerData : INetworkSerializable
    {
        public ulong Id;
        public ushort Length;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Length);
        }
    }
}