using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    
    void OnValidate()
    {
        if (_canvas == null)
        {
            _canvas = GetComponent<Canvas>();
        }
    }

    void OnEnable()
    {
        PlayerController.OnGameOveredEvent += HandleOnGameOvered;
    }

    void OnDisable()
    {
        PlayerController.OnGameOveredEvent -= HandleOnGameOvered;
    }
    
    void HandleOnGameOvered()
    {
        _canvas.enabled = true;
    }
}
