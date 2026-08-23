using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("ステータス")]
    public int MaxHP = 100;
    public int HP;
    public int Attak = 20;
    public int Defense = 0;
    public int BuildCost = 10;



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

    private void Start()
    {
        // 初期HPが未設定の場合はMaxHPで開始
        if (HP <= 0)
        {
            HP = MaxHP;
        }

        // 自分と子オブジェクトのRendererを取得
        renderers = GetComponentsInChildren<Renderer>();

        // 元の色を保存
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
        // 防御力を考慮した実質ダメージ計算（最低1ダメージ）
        int finalDamage = Mathf.Max(1, damage - Defense);
        HP -= finalDamage;

        Debug.Log(
            $"<color=cyan>[味方: {gameObject.name}] " +
            $"被ダメージ: {finalDamage} (残HP: {HP}/{MaxHP})</color>"
        );

        // ダメージ演出
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(DamageFlash());

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

        // 0.1秒待つ
        yield return new WaitForSeconds(damageFlashTime);

        // 元の色に戻す
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
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