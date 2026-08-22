using System.Collections.Generic;
using System.IO.Compression;
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

    [Header("攻撃設定")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackInterval = 1.0f;

    // 外部プロパティ
    public string RankName => rankName;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int AttackPower => attackPower;

    private NavMeshAgent agent;
    private GridManager gridManager;
    private Vector3 targetGoalPosition;

    // 攻撃対象（現在交戦中のモンスター）
    private Monster currentTargetMonster;
    // 破壊対象（最短の宝箱）
    private Treasure currentTargetTreasure;
    public void SetTargetTreasure(Treasure treasure)
    {
        currentTargetTreasure = treasure;
    }

    private float attackTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (currentTargetTreasure == null)
            return;

        // 宝箱との距離
        float distance = Vector3.Distance(
            transform.position,
            currentTargetTreasure.transform.position
        );

        // 宝箱に十分近づいた
        if (distance <= attackRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                AttackTreasure(currentTargetTreasure);
                attackTimer = 0f;
            }
        }
        // まだ遠いので宝箱に向かう
        else
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(
                    currentTargetTreasure.transform.position
                );
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

    private void AttackTreasure(Treasure target)
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
}