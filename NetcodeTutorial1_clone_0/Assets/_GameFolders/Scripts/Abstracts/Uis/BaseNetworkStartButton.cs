using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseNetworkStartButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] protected TMP_Text _text;

    void OnValidate()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }

        if (_text == null)
        {
            _text = GetComponentInChildren<TMP_Text>();
        }
    }

    void OnEnable()
    {
        _button.onClick.AddListener(HandleOnButtonClicked);
    }

    void OnDisable()
    {
        _button.onClick.RemoveListener(HandleOnButtonClicked);
    }

    protected abstract void HandleOnButtonClicked();
}