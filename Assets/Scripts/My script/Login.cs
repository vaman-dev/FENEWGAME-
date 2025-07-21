using UnityEngine;

public class Login : MonoBehaviour
{
    [Header("Login Info")]
    // [SerializeField] private string username;
    // [SerializeField] private string regNo;

    [Header("UI")]
    [SerializeField] private GameObject loginPanel;

    private void Start()
    {
        if (loginPanel == null)
        {
            Debug.LogError("[Login] Login Panel is not assigned in the inspector.");
            return;
        }

        loginPanel.SetActive(true);      // Show login panel at start
        Time.timeScale = 0f;             // Pause game until login is completed
    }

    public void OnLoginButtonClicked()
    {
        // if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(regNo))
        // {
        //     Debug.LogWarning("[Login] Username or Registration Number is empty.");
        //     return;
        // }

        // Debug.Log($"[Login] Username: {username}, RegNo: {regNo}");

        loginPanel.SetActive(false);     // Hide login panel
        Time.timeScale = 1f;             // Resume game
    }
}
