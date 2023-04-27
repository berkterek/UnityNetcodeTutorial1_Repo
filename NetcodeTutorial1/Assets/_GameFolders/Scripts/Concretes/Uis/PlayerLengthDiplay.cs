using TMPro;
using UnityEngine;

public class PlayerLengthDiplay : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    void OnValidate()
    {
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }
    }

    void OnEnable()
    {
        PlayerLengthHandler.OnLenghtValueChanged += HandleOnLengthValueChanged;
    }

    void OnDisable()
    {
        PlayerLengthHandler.OnLenghtValueChanged -= HandleOnLengthValueChanged;
    }
    
    void HandleOnLengthValueChanged(ushort value)
    {
        _text.SetText(value.ToString());
    }
}
