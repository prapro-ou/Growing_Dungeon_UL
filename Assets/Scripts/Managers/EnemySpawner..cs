using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private AdventurerData adventurerData;

    [Header("デフォルト敵設定")]
    [SerializeField] private AdventurerData.Rank defaultRank = AdventurerData.Rank.Iron;
    public Transform targetGoal;

    private void Start()
    {
        if (gridManager == null)
        {
            gridManager = Object.FindAnyObjectByType<GridManager>();
        }

        if (adventurerData == null)
        {
            adventurerData = Object.FindAnyObjectByType<AdventurerData>();
        }
    }

    /// <summary>
    /// デフォルトランクの敵を生成する
    /// </summary>
    public GameObject SpawnEnemy()
    {
        return SpawnEnemyByRank(defaultRank);
    }

    /// <summary>
    /// 指定されたランクの敵を生成し、IntruderNavMesh のパトロールを開始させる
    /// </summary>
    public GameObject SpawnEnemyByRank(AdventurerData.Rank rank)
    {
        if (adventurerData == null)
        {
            adventurerData = Object.FindAnyObjectByType<AdventurerData>();
            if (adventurerData == null)
            {
                Debug.LogError($"[{gameObject.name}] AdventurerData がシーン内に見つかりません！");
                return null;
            }
        }

        // 指定ランクのステータス情報（プレハブ等）を取得
        AdventurerData.RankStatus status = adventurerData.GetStatus(rank);

        if (status.prefab == null)
        {
            Debug.LogWarning($"[{gameObject.name}] {status.rankName} ({rank}) の プレハブ(prefab) が AdventurerData に設定されていません！");
            return null;
        }

        // プレハブから敵を生成
        GameObject enemy = Instantiate(status.prefab, transform.position, Quaternion.identity);
        Vector3 goalPos = targetGoal != null ? targetGoal.position : Vector3.zero;

        // IntruderNavMesh（移動・ステータス制御）を取得して発進させる
        IntruderNavMesh intruder = enemy.GetComponent<IntruderNavMesh>();
        if (intruder != null)
        {
            intruder.InitializeStatus(rank);
            intruder.StartPatrol(gridManager, goalPos);
        }
        else
        {
            UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && targetGoal != null)
            {
                agent.SetDestination(goalPos);
            }
        }

        return enemy;
    }
}