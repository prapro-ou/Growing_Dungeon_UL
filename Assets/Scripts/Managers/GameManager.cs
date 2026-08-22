using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("シーン設定")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private string gameClearSceneName = "GameClearScene";

    // プレイ時間
    public float PlayTime { get; private set; }

    // ゲーム終了済みか
    public bool IsGameFinished { get; private set; }

    private void Awake()
    {
        // GameManagerを1つだけにする
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // ゲーム中だけ時間を進める
        if (!IsGameFinished)
        {
            PlayTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void GameOver()
    {
        if (IsGameFinished)
            return;

        IsGameFinished = true;

        Debug.Log("<color=red>=== GAME OVER ===</color>");
        Debug.Log($"プレイ時間: {PlayTime:F1}秒");

        SceneManager.LoadScene(gameOverSceneName);
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    public void GameClear()
    {
        if (IsGameFinished)
            return;

        IsGameFinished = true;

        Debug.Log("<color=gold>=== GAME CLEAR ===</color>");
        Debug.Log($"プレイ時間: {PlayTime:F1}秒");

        SceneManager.LoadScene(gameClearSceneName);
    }
}
