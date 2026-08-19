using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class IntruderNavMesh : MonoBehaviour
{
    [Header("敵のランク設定")]
    [SerializeField] private AdventurerData.Rank currentRank = AdventurerData.Rank.Iron;

    [Header("現在のステータス（自動設定）")]
    [SerializeField] private string rankName;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int attackPower;
    [SerializeField] private float attackInterval;

    // 外部からステータスを確認・取得したい場合のプロパティ
    public string RankName => rankName;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int AttackPower => attackPower;
    public float AttackInterval => attackInterval;

    private NavMeshAgent agent;
    private GridManager gridManager;
    private Vector3 targetGoalPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// スポーン時や初期化時に呼んで、AdventurerData からステータスを読み込む
    /// </summary>
    public void InitializeStatus(AdventurerData.Rank rank)
    {
        currentRank = rank;

        // シーン内の AdventurerData からステータスを取得
        AdventurerData dataManager = Object.FindAnyObjectByType<AdventurerData>();
        if (dataManager != null)
        {
            AdventurerData.RankStatus status = dataManager.GetStatus(currentRank);

            rankName = status.rankName;
            maxHealth = status.maxHealth;
            currentHealth = maxHealth;
            moveSpeed = status.moveSpeed;
            attackPower = status.attackPower;
            attackInterval = status.attackInterval;

            // NavMeshAgent の移動速度にも自動反映
            if (agent != null)
            {
                agent.speed = moveSpeed;
            }

            Debug.Log($"[{gameObject.name}] {rankName} ステータス初期化完了 (HP:{maxHealth}, Speed:{moveSpeed})");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] シーン内に AdventurerData が見つかりませんでした。");
        }
    }

    /// <summary>
    /// EnemySpawner から呼ばれる巡回・ゴール移動開始処理
    /// </summary>
    public void StartPatrol(GridManager grid, Vector3 goalPosition)
    {
        this.gridManager = grid;
        this.targetGoalPosition = goalPosition;

        // まだ初期化されていない場合はデフォルトランクで初期化
        if (maxHealth == 0)
        {
            InitializeStatus(currentRank);
        }

        // ゴールへ向けて移動開始
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(targetGoalPosition);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] NavMeshAgent が有効でないか、NavMesh 上に配置されていません。");
        }
    }

    /// <summary>
    /// 被ダメージ処理
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[{rankName}] 被ダメージ: {damage} (残HP: {currentHealth}/{maxHealth})");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[{rankName}] 撃破されました！");
        Destroy(gameObject);
    }
}