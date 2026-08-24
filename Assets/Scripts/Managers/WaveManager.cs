using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    public enum GamePhase
    {
        InitialSetup,  // 最初の宝箱設置
        PrepPhase,     // Waveの建築
        WavePhase      // Waveの侵略
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
    [SerializeField] private GameObject buildMenu;
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
            Debug.LogError(
                $"[{gameObject.name}] EnemySpawner がシーン内に見つかりません！"
            );

            return false;
        }

        return true;
    }


    // =========================================================
    // 初期宝箱設置
    // =========================================================

    public void FinishInitialSetup()
    {
        if (currentPhase != GamePhase.InitialSetup)
            return;

        if (!IsInitialTreasureSetupComplete())
        {
            Debug.LogWarning(
                "メイン宝箱1個、サブ宝箱3個を設置してください。"
            );

            return;
        }

        Debug.Log("=== 初期宝箱設置完了 ===");

        currentWaveIndex = 0;

        // BuildModeをNoneにする
        if (buildManager != null)
        {
            buildManager.SetBuildMode(BuildMode.None);
        }

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


    // =========================================================
    // 初期宝箱の数を確認
    // =========================================================

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


    // =========================================================
    // 建築フェーズ開始
    // =========================================================

    [Header("DP設定")]
    [SerializeField] private int baseDpReward = 50; // 基準となるDP（50固定）

    public void EnterPrepPhase()
    {
        currentPhase = GamePhase.PrepPhase;

        Debug.Log(
            $"<color=green>=== 設置フェーズ開始 " +
            $"(Wave {currentWaveIndex + 1} の準備) ===</color>"
        );

        // Readyボタンを有効化
        if (readyButton != null)
        {
            readyButton.interactable = true;
        }

        // 設置メニューを表示
        if (buildMenu != null)
        {
            buildMenu.SetActive(true);
        }

        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayBuildBGM();
        }

        if (buildManager != null)
        {
            buildManager.UpdatePreviousWaveObjects();
        }

        onPrepPhaseStart?.Invoke();
        onPhaseChanged?.Invoke(currentPhase);
    }


    // =========================================================
    // Readyボタン → 侵略開始
    // =========================================================

    public void StartNextWave()
    {
        if (currentPhase == GamePhase.WavePhase)
            return;

        if (currentWaveIndex >= waveList.Count)
        {
            Debug.Log(
                "<color=gold>" +
                "すべてのウェーブをすでにクリアしています！" +
                "</color>"
            );

            return;
        }

        if (!EnsureSpawnerReference())
            return;


        // -----------------------------------------------------
        // 一旦Readyボタンを無効化
        // -----------------------------------------------------

        if (readyButton != null)
        {
            readyButton.interactable = false;
        }


        // -----------------------------------------------------
        // NavMeshを再構築
        // -----------------------------------------------------

        if (gridManager != null)
        {
            gridManager.RebuildNavMesh();
        }


        // -----------------------------------------------------
        // 宝箱へのアクセスチェック
        // -----------------------------------------------------

        if (!CanAccessAllTreasures())
        {
            Debug.LogWarning(
                "================================================\n" +
                "❌ 侵略を開始できません！\n" +
                "アクセスできない宝箱があります。\n" +
                "壁を確認してください。\n" +
                "================================================"
            );

            // Readyボタンをもう一度押せるようにする
            if (readyButton != null)
            {
                readyButton.interactable = true;
            }

            return;
        }


        // -----------------------------------------------------
        // ここまで来たら侵略開始可能
        // -----------------------------------------------------

        // 戦闘中は設置メニューを非表示
        if (buildMenu != null)
        {
            buildMenu.SetActive(false);
        }

        // 建築モード解除
        if (buildManager != null)
        {
            buildManager.ClearBuildSelection();
        }

        // プレビュー消去
        if (previewManager != null)
        {
            previewManager.ClearPreview();
        }

        StartCoroutine(RunWaveSequence());
    }


    // =========================================================
    // 宝箱へのアクセス可能判定
    // =========================================================

    private bool CanAccessAllTreasures()
    {
        Treasure[] treasures = FindObjectsByType<Treasure>(
            FindObjectsInactive.Exclude
        );

        if (treasures.Length == 0)
        {
            Debug.LogWarning("宝箱が1つも見つかりません。");
            return false;
        }

        // EnemySpawnerの位置 = 敵のスポーン地点
        Vector3 spawnPosition = spawner.transform.position;

        if (!NavMesh.SamplePosition(
            spawnPosition,
            out NavMeshHit startHit,
            0.5f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning(
                "敵のスポーン地点がNavMesh上にありません。"
            );

            return false;
        }

        foreach (Treasure treasure in treasures)
        {
            if (treasure == null)
                continue;

            bool accessible = false;

            // 宝箱の周囲を8方向チェック
            Vector3 center = treasure.transform.position;

            float checkRadius = 1.5f;

            Vector3[] checkPositions =
            {
                center + Vector3.forward * checkRadius,
                center + Vector3.back * checkRadius,
                center + Vector3.left * checkRadius,
                center + Vector3.right * checkRadius,

                center + (Vector3.forward + Vector3.right).normalized * checkRadius,
                center + (Vector3.forward + Vector3.left).normalized * checkRadius,
                center + (Vector3.back + Vector3.right).normalized * checkRadius,
                center + (Vector3.back + Vector3.left).normalized * checkRadius
            };

            foreach (Vector3 checkPosition in checkPositions)
            {
                // 近くのNavMeshを探す
                if (!NavMesh.SamplePosition(
                    checkPosition,
                    out NavMeshHit targetHit,
                    0.4f,
                    NavMesh.AllAreas))
                {
                    continue;
                }

                // スポーン地点からチェック地点まで経路を計算
                NavMeshPath path = new NavMeshPath();

                bool foundPath = NavMesh.CalculatePath(
                    startHit.position,
                    targetHit.position,
                    NavMesh.AllAreas,
                    path
                );

                if (foundPath &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    accessible = true;
                    break;
                }
            }

            if (!accessible)
            {
                Debug.LogWarning(
                    $"❌ 宝箱「{treasure.name}」へアクセスできません！"
                );

                return false;
            }

            Debug.Log(
                $"✅ 宝箱「{treasure.name}」へアクセス可能"
            );
        }

        Debug.Log(
            "<color=green>" +
            "✅ すべての宝箱へアクセス可能です。" +
            "</color>"
        );

        return true;
    }


    // =========================================================
    // GameManager互換
    // =========================================================

    public void StartWaveSystem()
    {
        StartNextWave();
    }


    // =========================================================
    // Wave実行
    // =========================================================

    private IEnumerator RunWaveSequence()
    {
        // 侵略開始時に通常表示へ
        if (buildManager != null)
        {
            buildManager.ResetObjectTransparency();
        }

        currentPhase = GamePhase.WavePhase;

        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayAttackBGM();
        }

        onWavePhaseStart?.Invoke();
        onPhaseChanged?.Invoke(currentPhase);


        WaveData currentWave = waveList[currentWaveIndex];

        Debug.Log(
            $"<color=cyan>" +
            $"=== {currentWave.waveName} " +
            $"(Wave {currentWaveIndex + 1}/{waveList.Count}) " +
            $"戦闘開始！ ===" +
            $"</color>"
        );


        // =====================================================
        // SubWave
        // =====================================================

        for (
            int subIndex = 0;
            subIndex < currentWave.subWaves.Count;
            subIndex++
        )
        {
            EnemySubWave subWave =
                currentWave.subWaves[subIndex];


            float spawnInterval =
                subWave.enemiesPerMinute > 0
                ? 60f / subWave.enemiesPerMinute
                : 1f;


            int spawnedCount = 0;


            while (
                spawnedCount <
                subWave.totalEnemiesToSpawn
            )
            {
                spawner.SpawnEnemyByRank(
                    subWave.enemyRank
                );

                spawnedCount++;


                if (
                    spawnedCount <
                    subWave.totalEnemiesToSpawn
                )
                {
                    yield return new WaitForSeconds(
                        spawnInterval
                    );
                }
            }


            if (
                subWave.delayBeforeNextSubWave > 0f
            )
            {
                yield return new WaitForSeconds(
                    subWave.delayBeforeNextSubWave
                );
            }
        }


        // =====================================================
        // 全敵生成完了
        // =====================================================

        Debug.Log(
            $"[{currentWave.waveName}] " +
            "全ての敵の生成が完了しました。" +
            "残敵の全滅を待っています..."
        );


        // フィールド上の敵が全滅するまで待機
        while (
            UnityEngine.Object.FindObjectsByType<
                IntruderNavMesh
            >().Length > 0
        )
        {
            yield return new WaitForSeconds(0.5f);
        }


        // =====================================================
        // Waveクリア
        // =====================================================

        Debug.Log(
            $"<color=yellow>" +
            $"=== Wave {currentWaveIndex + 1} クリア！ ===" +
            $"</color>"
        );


        currentWaveIndex++;


        if (currentWaveIndex < waveList.Count)
        {
            EnterPrepPhase();
        }
        else
        {
            Debug.Log(
                "<color=gold>" +
                "=== 全ウェーブ完全クリア！ ===" +
                "</color>"
            );

            onAllWavesCleared?.Invoke();

            GameManager.Instance.GameClear();
        }
    }
}