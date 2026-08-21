using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public enum GamePhase
    {
        PrepPhase, // 設置フェーズ（準備中）
        WavePhase  // 戦闘フェーズ（ウェーブ実行中）
    }

    [System.Serializable]
    public struct EnemySubWave
    {
        [Tooltip("このグループで出現させる敵のランク")]
        public AdventurerData.Rank enemyRank;

        [Tooltip("1分間（60秒）あたりに出現させる体数")]
        public float enemiesPerMinute;

        [Tooltip("このグループで出現させる合計体数")]
        public int totalEnemiesToSpawn;

        [Tooltip("次の敵グループが出現するまでの待ち時間（秒）")]
        public float delayBeforeNextSubWave;
    }

    [System.Serializable]
    public struct WaveData
    {
        [Tooltip("ウェーブ名")]
        public string waveName;

        [Tooltip("このウェーブ内で出現する敵グループのリスト")]
        public List<EnemySubWave> subWaves;
    }

    [Header("参照")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject buildMenu; // 設置メニュー全体のオブジェクト（MonsterMenuなど）

    [Header("ウェーブ設定")]
    [SerializeField] private List<WaveData> waveList = new List<WaveData>();

    [Header("フェーズ状態")]
    [SerializeField] private GamePhase currentPhase = GamePhase.PrepPhase;
    public int currentWaveIndex = 0;

    [Header("イベント（UI通知用など）")]
    public UnityEvent onPrepPhaseStart;
    public UnityEvent onWavePhaseStart;
    public UnityEvent onAllWavesCleared;

    private void Start()
    {
        EnsureSpawnerReference();
        EnterPrepPhase();
    }

    private bool EnsureSpawnerReference()
    {
        if (spawner == null)
        {
            spawner = Object.FindAnyObjectByType<EnemySpawner>();
        }

        if (spawner == null)
        {
            Debug.LogError($"[{gameObject.name}] EnemySpawner がシーン内に見つかりません！");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 設置フェーズに入る（Waveクリア時やゲーム開始時）
    /// </summary>
    public void EnterPrepPhase()
    {
        currentPhase = GamePhase.PrepPhase;
        Debug.Log($"<color=green>=== 設置フェーズ開始 (Wave {currentWaveIndex + 1} の準備) ===</color>");

        // Readyボタンを有効化
        if (readyButton != null)
        {
            readyButton.interactable = true;
        }

        // 設置メニューを再表示・有効化する
        if (buildMenu != null)
        {
            buildMenu.SetActive(true);
        }

        onPrepPhaseStart?.Invoke();
    }

    /// <summary>
    /// Readyボタンから呼び出されるメソッド（戦闘開始）
    /// </summary>
    public void StartNextWave()
    {
        if (currentPhase == GamePhase.WavePhase) return;

        if (currentWaveIndex >= waveList.Count)
        {
            Debug.Log("<color=gold>すべてのウェーブをすでにクリアしています！</color>");
            return;
        }

        if (!EnsureSpawnerReference()) return;

        if (readyButton != null)
        {
            readyButton.interactable = false;
        }

        // 戦闘中は設置メニューを非表示にする
        if (buildMenu != null)
        {
            buildMenu.SetActive(false);
        }

        StartCoroutine(RunWaveSequence());
    }

    /// <summary>
    /// GameManager 互換用の呼び出しメソッド
    /// </summary>
    public void StartWaveSystem()
    {
        StartNextWave();
    }

    private IEnumerator RunWaveSequence()
    {
        currentPhase = GamePhase.WavePhase;
        onWavePhaseStart?.Invoke();

        WaveData currentWave = waveList[currentWaveIndex];
        Debug.Log($"<color=cyan>=== {currentWave.waveName} (Wave {currentWaveIndex + 1}/{waveList.Count}) 戦闘開始！ ===</color>");

        for (int subIndex = 0; subIndex < currentWave.subWaves.Count; subIndex++)
        {
            EnemySubWave subWave = currentWave.subWaves[subIndex];

            float spawnInterval = subWave.enemiesPerMinute > 0 
                ? 60f / subWave.enemiesPerMinute 
                : 1f;

            int spawnedCount = 0;

            while (spawnedCount < subWave.totalEnemiesToSpawn)
            {
                spawner.SpawnEnemyByRank(subWave.enemyRank);
                spawnedCount++;

                if (spawnedCount < subWave.totalEnemiesToSpawn)
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            if (subWave.delayBeforeNextSubWave > 0f)
            {
                yield return new WaitForSeconds(subWave.delayBeforeNextSubWave);
            }
        }

        Debug.Log($"[{currentWave.waveName}] 全ての敵の生成が完了しました。残敵の全滅を待っています...");

        // フィールド上の敵（IntruderNavMesh）が全滅するまで待機
        // フィールド上の敵（IntruderNavMesh）が全滅するまで待機
    // フィールド上の敵（IntruderNavMesh）が全滅するまで待機
        while (Object.FindObjectsByType<IntruderNavMesh>().Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log($"<color=yellow>=== Wave {currentWaveIndex + 1} クリア！ ===</color>");

        currentWaveIndex++;

        if (currentWaveIndex < waveList.Count)
        {
            EnterPrepPhase();
        }
        else
        {
            Debug.Log("<color=gold>=== 全ウェーブ完全クリア！ ===</color>");
            onAllWavesCleared?.Invoke();
        }
    }
}