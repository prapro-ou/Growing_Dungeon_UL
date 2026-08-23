using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("モンスター設定")]
    [SerializeField] private MonsterData.MonsterType monsterType;

    public MonsterData.MonsterType MonsterType => monsterType;

    [Header("ステータス（MonsterDataから自動設定）")]
    public int MaxHP;
    public int HP;
    public int Attack;
    public int BuildCost;

    public float MoveSpeed;
    public float AttackRange;
    public float DetectionRange;
    public float AttackInterval;

    private MonsterData monsterData;

    private Tile tile;

    [Header("ダメージ演出")]
    [SerializeField] private float damageFlashTime = 0.1f;

    private Renderer[] renderers;
    private Color[] originalColors;
    private Coroutine damageFlashCoroutine;

    private MonsterView monsterView;

    // 自身が設置されたタイルを記憶
    public void SetTile(Tile tile)
    {
        this.tile = tile;

        monsterView = GetComponent<MonsterView>();
    }

    private void Awake()
    {
        monsterData = FindAnyObjectByType<MonsterData>();

        if (monsterData == null)
        {
            Debug.LogError(
                "シーン内にMonsterDataがありません"
            );

            return;
        }
    }

    private void Start()
    {
        monsterData = FindAnyObjectByType<MonsterData>();

        if (monsterData != null)
        {
            MonsterData.MonsterStatus status =
                monsterData.GetStatus(monsterType);

            MaxHP = status.maxHealth;
            HP = MaxHP;
            Attack = status.attackPower;
            BuildCost = status.buildCost;

            MoveSpeed = status.moveSpeed;
            AttackRange = status.attackRange;
            DetectionRange = status.detectionRange;
            AttackInterval = status.attackInterval;

            // NavMeshAgentの速度もモンスターごとに変更
            UnityEngine.AI.NavMeshAgent agent =
                GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (agent != null)
            {
                agent.speed = status.moveSpeed;
            }
        }

        // Renderer取得
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    /// <summary>
    /// モンスターがダメージを受ける処理
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 防御力なし
        HP -= damage;

        Debug.Log(
            $"<color=cyan>[味方: {gameObject.name}] " +
            $"被ダメージ: {damage} " +
            $"(残HP: {HP}/{MaxHP})</color>"
        );

        // ダメージ演出
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine =
            StartCoroutine(DamageFlash());

        if (HP <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        // 赤くする
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(
            damageFlashTime
        );

        // 元の色に戻す
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color =
                originalColors[i];
        }

        damageFlashCoroutine = null;
    }

    public void SetLookDirection(Vector3 targetPosition)
    {
        if (monsterView != null)
        {
            monsterView.FaceTarget(targetPosition);
        }
    }

    private void Die()
    {
        Debug.Log(
            $"<color=red>[味方: {gameObject.name}] " +
            $"は倒されて破壊されました！</color>"
        );

        if (tile != null)
        {
            tile.Type = TileType.Floor;
            tile.IsWalkable = true;
            tile.PlacedObject = null;
        }

        Destroy(gameObject);
    }
}