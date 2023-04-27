using Unity.Netcode;
using UnityEngine;

public class SnakeController : NetworkBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] Transform _transform;

    Camera _mainCamera;
    Vector3 _mouseInput = Vector3.zero;

    private void Initialize()
    {
        this.enabled = IsOwner;
        _mainCamera = Camera.main;
        _transform = transform;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    void Update()
    {
        if (!Application.isFocused) return;
        
        _mouseInput = (Vector2)Input.mousePosition;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
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
}