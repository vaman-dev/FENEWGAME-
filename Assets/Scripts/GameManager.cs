using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;

    [Header("Login Fields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField regNoInputField;

    [Header("Retry Control")]
    private int retryCount = 0;
    private const int maxRetries = 3;

    private Player player;
    private Spawner spawner;

    private float score;
    public float Score => score;

    private string playerName;
    private string regNo;

    private bool hasPostedScore = false;
    private const string baseUrl = "https://induction-backend.onrender.com/api/v1";

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();
        NewGame();
    }

    public void NewGame()
    {
        foreach (var obstacle in FindObjectsOfType<Obstacle>())
            Destroy(obstacle.gameObject);

        score = 0f;
        gameSpeed = initialGameSpeed;
        enabled = true;

        player?.gameObject.SetActive(true);
        spawner?.gameObject.SetActive(true);
        gameOverText?.gameObject.SetActive(false);
        retryButton?.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player?.gameObject.SetActive(false);
        spawner?.gameObject.SetActive(false);
        gameOverText?.gameObject.SetActive(true);

        retryCount++;
        retryButton?.gameObject.SetActive(retryCount < maxRetries);

        Debug.Log(retryCount < maxRetries
            ? $"🔁 Retry attempt {retryCount}/{maxRetries}"
            : "🚫 Max retries reached. Retry disabled.");

        SendScoreToBackend(); // 🔁 Always send current score
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    public void SetLoginInfo()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        playerName = nameInputField?.text.Trim();
        regNo = regNoInputField?.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(regNo))
        {
            Debug.LogWarning("⚠️ Name or RegNo is empty.");
        }
        else
        {
            Debug.Log($"✅ Player: {playerName}, RegNo: {regNo}");
        }
    }

    private void SendScoreToBackend()
    {
        if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(regNo))
        {
            Debug.LogWarning("⚠️ Cannot send score. Missing name or regNo.");
            return;
        }

        int currentScore = Mathf.FloorToInt(score);
        Debug.Log($"🎯 Sending score: {currentScore}");

        if (!hasPostedScore)
        {
            StartCoroutine(SendPostRequest(currentScore));
        }
        else
        {
            StartCoroutine(SendPatchRequest(currentScore));
        }
    }

    private IEnumerator SendPostRequest(int currentScore)
    {
        string url = baseUrl;

        ScoreData data = new ScoreData
        {
            name = playerName,
            ReqNo = regNo,
            score = currentScore
        };

        string json = JsonUtility.ToJson(data);
        byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("📡 Sending POST request...");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score POSTED successfully!");
            hasPostedScore = true;
        }
        else
        {
            Debug.LogError($"❌ POST failed: {request.responseCode} - {request.error}");
        }
    }

    private IEnumerator SendPatchRequest(int currentScore)
    {
        string patchUrl = $"{baseUrl}/{regNo}";

        ScoreUpdateData update = new ScoreUpdateData
        {
            score = currentScore
        };

        string json = JsonUtility.ToJson(update);
        byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(patchUrl, "PATCH");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("📡 Sending PATCH request...");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score PATCHED successfully!");
        }
        else
        {
            Debug.LogError($"❌ PATCH failed: {request.responseCode} - {request.error}");
        }
    }

    [System.Serializable]
    private class ScoreData
    {
        public string name;
        public string ReqNo;
        public int score;
    }

    [System.Serializable]
    private class ScoreUpdateData
    {
        public int score;
    }
}
