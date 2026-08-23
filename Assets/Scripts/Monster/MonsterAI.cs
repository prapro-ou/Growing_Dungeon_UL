using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    private enum MonsterState
    {
        Idle,           // 待機
        Chasing,        // 侵入者を追う
        Attacking,      // 侵入者を攻撃
        Returning,      // 初期位置へ戻る
        DefendingChest  // 宝箱を守る
    }

    [Header("レイヤー設定")]
    [SerializeField] private LayerMask intruderLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private float detectionRange;
    private float attackRange;
    private float attackInterval;

    private MonsterDetection detection;

    private NavMeshAgent agent;

    private Attack attack;

    private MonsterView monsterView;

    // モンスターが最初にいた場所
    private Vector3 initialPosition;

    // 現在狙っている侵入者
    private IntruderNavMesh currentTarget;

    // 現在の状態
    private MonsterState currentState = MonsterState.Idle;

    public void DefendChest(IntruderNavMesh attacker)
    {
        if (attacker == null)
            return;

        // 現在のターゲットを宝箱を攻撃している侵入者に変更
        currentTarget = attacker;

        // 現在の攻撃を中断
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        // Attack.csにもターゲットを渡す
        if (attack != null)
        {
            attack.SetTarget(attacker);
        }

        // 防衛状態へ
        currentState = MonsterState.DefendingChest;

        Debug.Log(
            $"[{gameObject.name}] メイン宝箱を攻撃している" +
            $"{attacker.gameObject.name}を迎撃します"
        );
    }

    private Quaternion originalRotation;

    private MonsterData monsterData;
    private Monster monster;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        detection = GetComponent<MonsterDetection>();
        monsterView = GetComponent<MonsterView>();
        attack = GetComponent<Attack>();
        monster = GetComponent<Monster>();

        if (monster == null)
        {
            Debug.LogError(
                $"{gameObject.name} にMonster.csがありません"
            );
            return;
        }

        detectionRange = monster.DetectionRange;
        attackRange = monster.AttackRange;
        attackInterval = monster.AttackInterval;

        agent.speed = monster.MoveSpeed;

        originalRotation = transform.rotation;
    }

    private void Start()
    {
        // 初期位置を記録
        initialPosition = transform.position;

        currentState = MonsterState.Idle;
    }

    private void Update()
    {
        switch (currentState)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;

            case MonsterState.Chasing:
                UpdateChasing();
                break;

            case MonsterState.Attacking:
                UpdateAttacking();
                break;

            case MonsterState.Returning:
                UpdateReturning();
                break;

            case MonsterState.DefendingChest:
                UpdateDefendingChest();
                break;
        }

        // 本体のRotationを固定
        transform.rotation = originalRotation;

        // 移動方向にスプライトを左右反転
        if (monsterView != null)
        {
            monsterView.UpdateDirection(agent.velocity);
        }

    }

    // =========================
    // 待機
    // =========================

    private void UpdateIdle()
    {
        IntruderNavMesh intruder = detection.FindIntruder();

        if (intruder != null)
        {
            Debug.Log("侵入者を発見！");

            currentTarget = intruder;

            if (attack != null)
            {
                attack.SetTarget(currentTarget);
            }

            currentState = MonsterState.Chasing;
        }
    }

    // =========================
    // 侵入者を追う
    // =========================

    private void UpdateChasing()
    {
        if (currentTarget == null)
        {
            Debug.Log("ターゲットを失った");

            currentState = MonsterState.Returning;
            return;
        }

        Debug.Log("侵入者を追跡中");

        float distance = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        if (distance <= attackRange)
        {
            agent.isStopped = true;

            currentState = MonsterState.Attacking;
            return;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(
                currentTarget.transform.position
            );
        }
    }

    // =========================
    // 攻撃
    // =========================

    private void UpdateAttacking()
    {
        if (currentTarget == null)
        {
            currentTarget = null;

            if (attack != null)
            {
                attack.ClearTarget();
            }

            currentState = MonsterState.Returning;
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        // 攻撃範囲から出た
        if (distance > attackRange)
        {
            currentState = MonsterState.Chasing;
            return;
        }

        // 攻撃そのものはAttack.csに任せる
    }

    // =========================
    // 初期位置へ戻る
    // =========================

    private void UpdateReturning()
    {
        // 戻っている途中でも索敵する
        IntruderNavMesh intruder = detection.FindIntruder();

        if (intruder != null)
        {
            currentTarget = intruder;
        if (attack != null)
        {
            attack.SetTarget(currentTarget);
        }

        currentState = MonsterState.Chasing;
        return;
        }

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(initialPosition);

        // 初期位置に到着
        if (!agent.pathPending &&
            agent.remainingDistance <= 0.2f)
        {
            agent.isStopped = true;
            currentState = MonsterState.Idle;
        }
    }

    // =========================
    // 宝箱防衛
    // =========================

    private void UpdateDefendingChest()
    {
        // 攻撃対象がいなくなった
        if (currentTarget == null)
        {
            if (attack != null)
            {
                attack.ClearTarget();
            }

            currentState = MonsterState.Returning;
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        // 攻撃範囲に入った
        if (distance <= attackRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            return;
        }

        // まだ遠い → 攻撃対象へ向かう
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(
                currentTarget.transform.position
            );
        }
    }

    // =========================
    // 索敵範囲の表示
    // =========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}