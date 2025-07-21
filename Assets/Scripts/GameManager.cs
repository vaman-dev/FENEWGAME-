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
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;

    [Header("Login Fields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField regNoInputField;

    private Player player;
    private Spawner spawner;

    private float score;
    public float Score => score;

    private string playerName;
    private string regNo;

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
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
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        score = 0f;
        gameSpeed = initialGameSpeed;
        enabled = true;

        player?.gameObject.SetActive(true);
        spawner?.gameObject.SetActive(true);
        gameOverText?.gameObject.SetActive(false);
        retryButton?.gameObject.SetActive(false);

        UpdateHiscore();
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player?.gameObject.SetActive(false);
        spawner?.gameObject.SetActive(false);
        gameOverText?.gameObject.SetActive(true);
        retryButton?.gameObject.SetActive(true);

        UpdateHiscore();
        SendScoreToBackend(); // ✅ Send score to backend
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

    // ✅ Called from Login Button
    public void SetLoginInfo()
    {
        playerName = nameInputField?.text.Trim();
        regNo = regNoInputField?.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(regNo))
        {
            Debug.LogWarning("⚠️ Player name or registration number is empty.");
        }
        else
        {
            Debug.Log($"✅ Player: {playerName}, RegNo: {regNo}");
        }
    }

    // ✅ Score sending function
    private void SendScoreToBackend()
    {
        if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(regNo))
        {
            Debug.LogWarning("⚠️ Cannot send score. Missing name or regNo.");
            return;
        }

        StartCoroutine(SendScoreRequest());
    }

    private IEnumerator SendScoreRequest()
    {
        string url = "https://induction-backend.onrender.com/api/v1/"; // 🔁 Replace with your backend endpoint

        ScoreData data = new ScoreData
        {
            name = playerName,
            ReqNo = regNo,
            score = Mathf.FloorToInt(score)
        };

        string json = JsonUtility.ToJson(data);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("📡 Sending data to backend...");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score sent successfully!");
        }
        else
        {
            Debug.LogError($"❌ Failed to send score: {request.error}");
        }
    }

    [System.Serializable]
    private class ScoreData
    {
        public string name;
        public string ReqNo;
        public int score;
    }
}
