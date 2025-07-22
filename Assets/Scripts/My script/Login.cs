using UnityEngine;

public class Login : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject loginPanel;

    private void Start()
    {
        if (loginPanel == null)
        {
            Debug.LogError("[Login] Login Panel is not assigned in the inspector.");
            return;
        }

        loginPanel.SetActive(true);  
        Time.timeScale = 0f;         
    }

    public void OnLoginButtonClicked()
    {
        loginPanel.SetActive(false);
        Time.timeScale = 1f;      
    }
}
