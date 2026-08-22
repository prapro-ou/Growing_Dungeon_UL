using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private AdventurerData adventurerData;

    [Header("デフォルト敵設定")]
    [SerializeField] private AdventurerData.Rank defaultRank = AdventurerData.Rank.Iron;

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
        GameObject enemy = Instantiate (
            status.prefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        // 一番近い宝箱を探す
        Treasure nearestTreasure = FindNearestTreasure(enemy.transform.position);

        if (nearestTreasure == null)
        {
            Debug.LogWarning("敵の目的地になる宝箱がありません");
            return enemy;
        }

        // 宝箱を目的地にする
        Vector3 goalPos = nearestTreasure.transform.position;

        // IntruderNavMesh（移動・ステータス制御）を取得して発進させる
        IntruderNavMesh intruder = enemy.GetComponent<IntruderNavMesh>();

        if (intruder != null)
        {
            intruder.InitializeStatus(rank);
            intruder.SetTargetTreasure(nearestTreasure);
            intruder.StartPatrol(gridManager, goalPos);
        }

        return enemy;
    }

    /// 一番近い宝箱を探す関数
    private Treasure FindNearestTreasure(Vector3 enemyPosition)
    {
        Treasure[] treasures = FindObjectsByType<Treasure>(
            FindObjectsInactive.Exclude
        );

        Treasure nearestTreasure = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Treasure treasure in treasures)
        {
            if (treasure == null)
                continue;

            float distance = Vector3.Distance(
                enemyPosition,
                treasure.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTreasure = treasure;
            }
        }

        return nearestTreasure;
    }
}