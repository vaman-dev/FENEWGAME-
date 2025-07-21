using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class VirtualKeyboardManager : MonoBehaviour
{
    public static VirtualKeyboardManager Instance { get; private set; }

    [Header("Keyboard Targets")]
    [SerializeField] private List<KeyboardTarget> keyboardTargets = new List<KeyboardTarget>();

    private TMP_InputField currentInputField;
    private GameObject currentPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

   private void Start()
{
    // Disable all keyboard panels at the beginning
    foreach (var target in keyboardTargets)
    {
        if (target.keyboardPanel != null)
            target.keyboardPanel.SetActive(false);

        if (target.inputField != null)
        {
            string labelCopy = target.label; // Prevent closure issues
            target.inputField.onSelect.AddListener(_ => ActivateKeyboardFor(labelCopy));
        }
    }
}


   public void ActivateKeyboardFor(string label)
{
    foreach (var target in keyboardTargets)
    {
        if (target.label == label)
        {
            currentInputField = target.inputField;
            currentPanel = target.keyboardPanel;

            if (currentPanel != null)
            {
                currentPanel.SetActive(true);
                Debug.Log($"[Keyboard] Canvas panel activated for: {label}");
            }

            Debug.Log($"[Keyboard] Activated for: {label}");
            return;
        }
    }

    Debug.LogWarning($"[Keyboard] No matching input field for label: {label}");
}

    public void PressKey(string key)
    {
        if (currentInputField == null) return;

        switch (key)
        {
            case "SPACE":
                currentInputField.text += " ";
                break;

            case "BACKSPACE":
                if (currentInputField.text.Length > 0)
                    currentInputField.text = currentInputField.text.Substring(0, currentInputField.text.Length - 1);
                break;

            case "ENTER":
                if (currentPanel != null)
                    currentPanel.SetActive(false);
                break;

            default:
                currentInputField.text += key;
                break;
        }
    }
}

[System.Serializable]
public class KeyboardTarget
{
    public string label;
    public GameObject keyboardPanel;
    public TMP_InputField inputField;
}
