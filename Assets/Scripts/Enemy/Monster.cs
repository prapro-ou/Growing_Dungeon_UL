using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("ステータス")]
    public int MaxHP = 100;
    public int HP;
    public int Attak = 20;
    public int Defense = 0;
    public int BuildCost = 10;

    private void Start()
    {
        // 初期HPが未設定の場合はMaxHPで開始
        if (HP <= 0)
        {
            HP = MaxHP;
        }
    }

    /// <summary>
    /// モンスターがダメージを受ける処理（戦闘後もHPは削れたまま維持）
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 防御力を考慮した実質ダメージ計算（最低1ダメージ）
        int finalDamage = Mathf.Max(1, damage - Defense);
        HP -= finalDamage;

        Debug.Log($"<color=cyan>[味方: {gameObject.name}] 被ダメージ: {finalDamage} (残HP: {HP}/{MaxHP})</color>");

        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>[味方: {gameObject.name}] は倒されて破壊されました！</color>");
        Destroy(gameObject);
    }
}