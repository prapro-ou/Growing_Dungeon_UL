using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class IntruderNavMesh : MonoBehaviour
{   
    [Header("DP設定")]
    [SerializeField] private int baseRewardDP = 1; 

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
    private float treasureAttackRange = 1.8f;
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

    private IntruderView intruderView;

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

    // モンスター攻撃モーション関係
    private bool isAttacking = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        intruderView = GetComponent<IntruderView>();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private void Start()
    {
        // 敵ごとにランダムな移動間隔を設定
        wanderChangeInterval = Random.Range(2f, 5f);

        // 最初の移動開始タイミングもずらす
        wanderTimer = Random.Range(0f, wanderChangeInterval);
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

        if (intruderView != null)
        {
            intruderView.UpdateDirection(agent.velocity);
        }
    }

    private void UpdateTreasureHunter()
    {
        // 現在の宝箱が破壊された
        if (currentTargetTreasure == null)
        {
            currentTargetTreasure = FindNearestTreasure();

            if (currentTargetTreasure == null)
            {
                // 宝箱がもうない
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                return;
            }

            Debug.Log(
                $"[{gameObject.name}] 次の宝箱へ向かいます: " +
                $"{currentTargetTreasure.gameObject.name}"
            );
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
        if (distance <= treasureAttackRange)
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

            // 新しい宝箱を見つけたらすぐ向かう
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTargetTreasure.transform.position);
            }
        }

        float treasureDistance = Vector3.Distance(
            transform.position,
            currentTargetTreasure.transform.position
        );

        // 宝箱が攻撃範囲内
        if (treasureDistance <= treasureAttackRange)
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
        // =========================
        // ① 近くのモンスターを探す
        // =========================
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

            // モンスターが近くにいる → 近づく
            if (monsterDistance <= attackRange * 3f)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(
                        nearestMonster.transform.position
                    );
                }

                return;
            }
        }

        // =========================
        // ② モンスターが近くにいない
        //    → 宝箱を探す
        // =========================

        Treasure nearestTreasure = FindNearestTreasure();

        if (nearestTreasure != null)
        {
            float treasureDistance = Vector3.Distance(
                transform.position,
                nearestTreasure.transform.position
            );

            // 宝箱が攻撃範囲内
            if (treasureDistance <= treasureAttackRange)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackInterval)
                {
                    AttackTreasure(nearestTreasure);
                    attackTimer = 0f;
                }

                return;
            }

            // 宝箱が近くにある → 宝箱へ向かう
            if (treasureDistance <= attackRange * 3f)
            {
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(
                        nearestTreasure.transform.position
                    );
                }

                return;
            }
        }

        // =========================
        // ③ 何も近くにいない
        //    → ランダム移動
        // =========================

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
        float shortestPathDistance = Mathf.Infinity;

        NavMeshHit startHit;

        if (!NavMesh.SamplePosition(
            transform.position,
            out startHit,
            2.0f,
            NavMesh.AllAreas))
        {
            return null;
        }

        foreach (Treasure treasure in treasures)
        {
            if (treasure == null)
                continue;

            NavMeshHit treasureHit;

            if (!NavMesh.SamplePosition(
                treasure.transform.position,
                out treasureHit,
                2.0f,
                NavMesh.AllAreas))
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();

            bool foundPath = NavMesh.CalculatePath(
                startHit.position,
                treasureHit.position,
                NavMesh.AllAreas,
                path
            );

            if (!foundPath || path.status != NavMeshPathStatus.PathComplete)
                continue;

            float pathDistance = 0f;

            for (int i = 1; i < path.corners.Length; i++)
            {
                pathDistance += Vector3.Distance(
                    path.corners[i - 1],
                    path.corners[i]
                );
            }

            if (pathDistance < shortestPathDistance)
            {
                shortestPathDistance = pathDistance;
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
        float shortestPathDistance = Mathf.Infinity;

        // 敵自身の位置をNavMesh上に補正
        NavMeshHit startHit;

        if (!NavMesh.SamplePosition(
            transform.position,
            out startHit,
            2.0f,
            NavMesh.AllAreas))
        {
            return null;
        }

        foreach (Monster monster in monsters)
        {
            if (monster == null)
                continue;

            // モンスターの位置をNavMesh上に補正
            NavMeshHit monsterHit;

            if (!NavMesh.SamplePosition(
                monster.transform.position,
                out monsterHit,
                2.0f,
                NavMesh.AllAreas))
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();

            bool foundPath = NavMesh.CalculatePath(
                startHit.position,
                monsterHit.position,
                NavMesh.AllAreas,
                path
            );

            if (!foundPath || path.status != NavMeshPathStatus.PathComplete)
                continue;

            float pathDistance = 0f;

            for (int i = 1; i < path.corners.Length; i++)
            {
                pathDistance += Vector3.Distance(
                    path.corners[i - 1],
                    path.corners[i]
                );
            }

            if (pathDistance < shortestPathDistance)
            {
                shortestPathDistance = pathDistance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }

    private void AttackMonster(Monster target)
    {
        if (target == null || isAttacking)
            return;

        StartCoroutine(AttackMonsterCoroutine(target));
    }

    private IEnumerator AttackMonsterCoroutine(Monster target)
    {
        isAttacking = true;

        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        // 攻撃中は移動停止
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        // 自分自身の攻撃モーション
        if (intruderView != null)
        {
            intruderView.FaceTarget(
                target.transform.position
            );

            yield return StartCoroutine(
                intruderView.PlayAttack(
                    target.transform.position
                )
            );
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        // モーション終了後にダメージ
        if (target != null)
        {
            target.TakeDamage(attackPower);
        }

        // 攻撃間隔
        yield return new WaitForSeconds(attackInterval);

        isAttacking = false;
    }

    private void AttackTreasure(Treasure target)
    {
        if (target == null || isAttacking)
            return;

        Debug.Log("★ 宝箱攻撃開始");

        StartCoroutine(AttackTreasureCoroutine(target));
    }

    private IEnumerator AttackTreasureCoroutine(Treasure target)
    {
        isAttacking = true;

        Debug.Log("★ 宝箱攻撃コルーチン開始");

        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        // 移動停止
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        // 攻撃方向を向く
        if (intruderView != null)
        {
            Debug.Log("★ PlayAttackを実行");

            intruderView.FaceTarget(
                target.transform.position
            );

            yield return StartCoroutine(
                intruderView.PlayAttack(
                    target.transform.position
                )
            );

            Debug.Log("★ PlayAttack終了");
        }

        // モーション終了後にダメージ
        if (target != null)
        {
            target.TakeDamage(attackPower);
        }

        yield return new WaitForSeconds(attackInterval);

        isAttacking = false;
    }

    /// ランダム移動先を決める関数
    private void SetRandomWanderTarget()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomDirection,
            out hit,
            10f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
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

        Debug.Log(
            $"<color=yellow>[敵: {rankName}] 被ダメージ: {damage} " +
            $"(残HP: {currentHealth}/{maxHealth})</color>"
        );

        // ダメージ演出
        if (intruderView != null)
        {
            intruderView.PlayDamageFlash();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>[敵: {rankName}] 撃破されました！</color>");
        
        int rewardDP = 1;
        switch (currentRank)
        {
            case AdventurerData.Rank.Iron:
                rewardDP = 1;
                break;
            case AdventurerData.Rank.Bronze:
                rewardDP = 2;
                break;
            case AdventurerData.Rank.Silver:
                rewardDP = 5;
                break;
            case AdventurerData.Rank.Gold:
                rewardDP = 10;
                break;
            case AdventurerData.Rank.Platinum:
                rewardDP = 20;
                break;
            case AdventurerData.Rank.Emerald:
                rewardDP = 35;
                break;
            case AdventurerData.Rank.Diamond:
                rewardDP = 50;
                break;
            case AdventurerData.Rank.Master:
                rewardDP = 75;
                break;
            case AdventurerData.Rank.Grandmaster:
                rewardDP = 100;
                break;
            case AdventurerData.Rank.Challenger:
                rewardDP = 150;
                break;
        }

        DungeonPointManager dpManager = FindFirstObjectByType<DungeonPointManager>();
        if (dpManager != null)
        {
            dpManager.AddDP(rewardDP);
        }

        Destroy(gameObject);
    }
}