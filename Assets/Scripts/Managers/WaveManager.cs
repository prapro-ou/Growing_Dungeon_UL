using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public enum GamePhase
    {
        InitialSetup,  // 最初の宝箱設置
        PrepPhase,     // Waveの建築
        WavePhase    // Waveの侵略
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
    [SerializeField] private GridManager gridManager;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private PreviewManager previewManager;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject buildMenu; // 設置メニュー全体のオブジェクト（MonsterMenuなど）
    [SerializeField] private GameObject treasureMenu;

    [Header("ウェーブ設定")]
    [SerializeField] private List<WaveData> waveList = new List<WaveData>();

    [Header("フェーズ状態")]
    [SerializeField] public GamePhase currentPhase = GamePhase.InitialSetup;
    public int currentWaveIndex = 0;

    [Header("イベント（UI通知用など）")]
    public UnityEvent onPrepPhaseStart;
    public UnityEvent onWavePhaseStart;
    public UnityEvent onAllWavesCleared;

    // フェーズが変更されたときに通知
    public event Action<GamePhase> onPhaseChanged;

    private void Start()
    {
        EnsureSpawnerReference();

        // ゲーム開始時は宝箱設置フェーズ
        currentPhase = GamePhase.InitialSetup;

        Debug.Log("=== 初期宝箱設置フェーズ開始 ===");

        // Readyボタンはまだ使えない
        if (readyButton != null)
        {
            readyButton.interactable = false;
        }

        // 建築メニューもまだ使えない
        if (buildMenu != null)
        {
            buildMenu.SetActive(false);
        }

        onPhaseChanged?.Invoke(currentPhase);
    }

    private bool EnsureSpawnerReference()
    {
        if (spawner == null)
        {
            spawner = UnityEngine.Object.FindAnyObjectByType<EnemySpawner>();
        }

        if (spawner == null)
        {
            Debug.LogError($"[{gameObject.name}] EnemySpawner がシーン内に見つかりません！");
            return false;
        }

        return true;
    }

    // 宝箱設置完了後呼び出す
    public void FinishInitialSetup()
    {
        if (currentPhase != GamePhase.InitialSetup)
            return;

        if (!IsInitialTreasureSetupComplete())
        {
            Debug.LogWarning("メイン宝箱1個、サブ宝箱3個を設置してください。");
            return;
        }

        Debug.Log("=== 初期宝箱設置完了 ===");

        currentWaveIndex = 0;

        // メニューボタンの表示変更
        if (buildMenu != null)
        {
            buildMenu.SetActive(true);
        }
        if (treasureMenu != null)
        {
            treasureMenu.SetActive(false);
        }


        EnterPrepPhase();
    }

    // 宝箱の設置上限を確かめる
    private bool IsInitialTreasureSetupComplete()
    {
        Treasure[] treasures = FindObjectsByType<Treasure>(
            FindObjectsInactive.Exclude
        );

        int mainCount = 0;
        int subCount = 0;

        foreach (Treasure treasure in treasures)
        {
            if (treasure.isMainTreasure)
            {
                mainCount++;
            }
            else
            {
                subCount++;
            }
        }

        return mainCount == 1 && subCount == 3;
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
        onPhaseChanged?.Invoke(currentPhase);
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
        
        // 壁の配置に合わせて NavMesh を再構築
        if (gridManager != null)
        {
            gridManager.RebuildNavMesh();
        }

        // 準備完了：建築モード解除＆プレビュー消去
        if (buildManager != null) buildManager.ClearBuildSelection();
        if (previewManager != null) previewManager.ClearPreview();

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
        onPhaseChanged?.Invoke(currentPhase);

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
        while (UnityEngine.Object.FindObjectsByType<IntruderNavMesh>().Length > 0)
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