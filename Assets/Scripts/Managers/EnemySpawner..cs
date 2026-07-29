using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成する敵のプレハブ")]
    public GameObject enemyPrefab;

    [Header("敵が目指すゴール")]
    public Transform targetGoal;

    [Header("生成間隔（秒）")]
    public float spawnInterval = 2.0f;

    private float timer = 0f;
    private bool isSpawning = false; // スポーン中かどうかを管理するフラグ

    void Update()
    {
        // GameManager から StartSpawning() が呼ばれるまでスポーンしない
        if (!isSpawning) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // タイマーリセット
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // スポーン位置に敵を生成
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // 敵のNavMeshAgentにゴールを設定する
        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && targetGoal != null)
        {
            agent.SetDestination(targetGoal.position);
        }
    }

    // GameManager から呼び出す関数（必ずクラスの内部 {} に入れます）
    public void StartSpawning()
    {
        isSpawning = true;
        timer = 0f; // 開始と同時にすぐスポーン処理を始められるようにリセット
        Debug.Log("敵のスポーンを開始しました");
    }

    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("敵のスポーンを停止しました");
    }
} // ★ クラスの閉じカッコは一番最後！