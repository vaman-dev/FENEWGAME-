using TMPro;
using UnityEngine;

public class VirtualKeyboardManager : MonoBehaviour
{
    public static VirtualKeyboardManager Instance;

    [SerializeField] private TMP_InputField targetInputField;

    private void Awake()
    {
        Instance = this;
    }

    public void PressKey(string key)
    {
        if (targetInputField == null)
        {
            Debug.LogWarning("No Input Field assigned.");
            return;
        }

        switch (key)
        {
            case "SPACE":
                targetInputField.text += " ";
                break;

            case "BACKSPACE":
                if (targetInputField.text.Length > 0)
                    targetInputField.text = targetInputField.text.Substring(0, targetInputField.text.Length - 1);
                break;

            default:
                targetInputField.text += key;
                break;
        }
    }
}
