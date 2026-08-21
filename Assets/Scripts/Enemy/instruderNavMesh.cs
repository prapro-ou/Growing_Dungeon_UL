using System.Collections.Generic;
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
    [SerializeField] private float attackInterval = 1.0f;

    // 外部プロパティ
    public string RankName => rankName;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int AttackPower => attackPower;
    public float AttackInterval => attackInterval;

    private NavMeshAgent agent;
    private GridManager gridManager;
    private Vector3 targetGoalPosition;

    // 攻撃対象（現在交戦中のモンスター）
    private Monster currentTargetMonster;
    private float attackTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // モンスターと交戦中の場合
        if (currentTargetMonster != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                AttackMonster(currentTargetMonster);
                attackTimer = 0f;
            }
        }
        else
        {
            // モンスターが倒されるか居なくなったら、ゴールへの移動を再開
            if (agent != null && agent.isOnNavMesh && agent.isStopped)
            {
                agent.isStopped = false;
                if (targetGoalPosition != Vector3.zero)
                {
                    agent.SetDestination(targetGoalPosition);
                }
            }
        }
    }

    private void AttackMonster(Monster target)
    {
        if (target != null)
        {
            target.TakeDamage(attackPower);
        }
    }

    /// <summary>
    /// スポーン時や初期化時に呼んで、AdventurerData からステータスを読み込む
    /// </summary>
    public void InitializeStatus(AdventurerData.Rank rank)
    {
        currentRank = rank;

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

            if (agent != null)
            {
                agent.speed = moveSpeed;
            }

            Debug.Log($"[{gameObject.name}] {rankName} ステータス初期化完了 (HP:{maxHealth}, Atk:{attackPower}, Int:{attackInterval}s)");
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

        if (maxHealth == 0)
        {
            InitializeStatus(currentRank);
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetGoalPosition);
        }
    }

    /// <summary>
    /// 被ダメージ処理（HPは戦闘後も削れたまま維持）
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"<color=yellow>[敵: {rankName}] 被ダメージ: {damage} (残HP: {currentHealth}/{maxHealth})</color>");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>[敵: {rankName}] 撃破されました！</color>");
        Destroy(gameObject);
    }

    // 2D当たり判定：モンスターの攻撃範囲に入ったら足を止めて戦闘開始
    private void OnTriggerEnter2D(Collider2D other)
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)
        {
            currentTargetMonster = monster;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true; // 足を止めて殴り合い
            }
        }
    }

    // モンスターの範囲から出た場合（またはモンスターが破壊された場合）
    private void OnTriggerExit2D(Collider2D other)
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null && currentTargetMonster == monster)
        {
            currentTargetMonster = null;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false; // 移動再開
            }
        }
    }
}