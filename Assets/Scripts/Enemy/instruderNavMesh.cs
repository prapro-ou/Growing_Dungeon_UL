using Unity.VisualScripting;
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
    [SerializeField] private AdventurerData.EnemyAIType aiType;

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

    // ランダム移動AI
    private Vector3 wanderTarget;
    private float wanderTimer = 0f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float wanderChangeInterval = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (aiType)
        {
            case AdventurerData.EnemyAIType.TreasureHunter:
            UpdateTreasureHunter();
            break;

        case AdventurerData.EnemyAIType.Aggressive:
            UpdateAggressive();
            break;

        case AdventurerData.EnemyAIType.Wanderer:
            UpdateWanderer();
            break;
        }
    }

    private void UpdateTreasureHunter()
    {
        if (currentTargetTreasure == null)
        {
            currentTargetTreasure = FindNearestTreasure();

            if (currentTargetTreasure == null)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                return;
            }
        }

        // 一番近いモンスターを探す
        Monster nearestMonster = FindNearestMonster();

        if (nearestMonster != null)
        {
            float monsterDistance = Vector3.Distance(
                transform.position,
                nearestMonster.transform.position
            );

            // モンスターが攻撃範囲内ならモンスターを攻撃
            if (monsterDistance <= attackRange)
            {
                agent.isStopped = true;

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackInterval)
                {
                    AttackMonster(nearestMonster);
                    attackTimer = 0f;
                }
                return;
            }
        }

        // モンスターが近くにいない場合は宝箱に向かう
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

    private void UpdateAggressive()
    {
        // 一番近いモンスターを探す
        Monster nearestMonster = FindNearestMonster();

        if (nearestMonster != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                nearestMonster.transform.position
            );

            // モンスターが攻撃範囲内
            if (distance <= attackRange)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackInterval)
                {
                    AttackMonster(nearestMonster);
                    attackTimer = 0f;
                }

                return;
            }

            // モンスターが遠い → モンスターへ向かう
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(
                    nearestMonster.transform.position
                );
            }

            return;
        }

        // モンスターが全部いなくなった
        // 一番近い宝箱を探す
        if (currentTargetTreasure == null)
        {
            currentTargetTreasure = FindNearestTreasure();

            if (currentTargetTreasure == null)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                return;
            }
        }

        float treasureDistance = Vector3.Distance(
            transform.position,
            currentTargetTreasure.transform.position
        );

        // 宝箱が攻撃範囲内
        if (treasureDistance <= attackRange)
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
        // 宝箱へ向かう
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

    private void UpdateWanderer()
    {
        // 近くのモンスターを探す
        Monster nearestMonster = FindNearestMonster();

        if (nearestMonster != null)
        {
            float monsterDistance = Vector3.Distance(
                transform.position,
                nearestMonster.transform.position
            );

            // モンスターが攻撃範囲内
            if (monsterDistance <= attackRange)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackInterval)
                {
                    AttackMonster(nearestMonster);
                    attackTimer = 0f;
                }

                return;
            }

            // モンスターが少し遠い場合はモンスターへ向かう
            if (monsterDistance <= attackRange * 3f)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(nearestMonster.transform.position);
                }

                return;
            }
        }

        // モンスターが近くにいない → ランダム移動
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderChangeInterval)
        {
            SetRandomWanderTarget();
            wanderTimer = 0f;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            if (!agent.hasPath || agent.remainingDistance <= 0.5f)
            {
                SetRandomWanderTarget();
            }
        }
    }



    private Treasure FindNearestTreasure()
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
                transform.position,
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

    private Monster FindNearestMonster()
    {
        Monster[] monsters = FindObjectsByType<Monster>(
            FindObjectsInactive.Exclude
        );

        Monster nearestMonster = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Monster monster in monsters)
        {
            if (monster == null)
                continue;
            
            float distance = Vector3.Distance(
                transform.position,
                monster.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
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

    /// ランダム移動先を決める関数
    private void SetRandomWanderTarget()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 randomPosition = transform.position +
                             Random.insideUnitSphere * wanderRadius;

        randomPosition.y = transform.position.y;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomPosition,
            out hit,
            wanderRadius,
            NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
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
            aiType = status.aiType;

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