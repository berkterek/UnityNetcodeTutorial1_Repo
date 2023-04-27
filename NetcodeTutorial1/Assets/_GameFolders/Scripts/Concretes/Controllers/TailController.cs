using UnityEngine;

public class TailController : MonoBehaviour
{
    [SerializeField] float _delayTime = 0.1f;
    [SerializeField] float _distance = 0.3f;
    [SerializeField] float _moveStep = 10f;
    [SerializeField] Transform _transform;

    Vector3 _targetPosition;


    public Transform FollowTransform { get; set; }
    public Transform NetworkOwner { get; set; }

    void OnValidate()
    {
        if (_transform == null)
        {
            _transform = transform;
        }
    }

    void Update()
    {
        _targetPosition = FollowTransform.position - FollowTransform.forward * _distance;
        Vector3 currentPosition = _transform.position;
        _targetPosition += (currentPosition - _targetPosition) * _delayTime;
        _targetPosition.z = 0f;

        currentPosition = Vector3.Lerp(currentPosition, _targetPosition, Time.deltaTime * _moveStep);
        _transform.position = currentPosition;
    }
}