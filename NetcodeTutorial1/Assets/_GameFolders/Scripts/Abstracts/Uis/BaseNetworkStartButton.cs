using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseNetworkStartButton : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] protected TMP_Text _text;
    [SerializeField] BaseNetworkStartButton[] _networkButtons;

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

    protected virtual void HandleOnButtonClicked()
    {
        foreach (var networkButton in _networkButtons)
        {
            networkButton.gameObject.SetActive(false);
        }
        
        this.gameObject.SetActive(false);
    }
}