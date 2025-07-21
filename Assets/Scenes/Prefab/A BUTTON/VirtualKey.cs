using UnityEngine;
using UnityEngine.UI;

public class VirtualKey : MonoBehaviour
{
    public string keyValue; // "A", "B", "SPACE", "BACKSPACE", etc.

    private Button keyButton;

    private void Awake()
    {
        keyButton = GetComponent<Button>();
        keyButton.onClick.AddListener(OnKeyPressed);
    }

    private void OnKeyPressed()
    {
        if (VirtualKeyboardManager.Instance != null)
        {
            VirtualKeyboardManager.Instance.PressKey(keyValue);
        }
    }
}
