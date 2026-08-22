using UnityEditor.ShaderGraph;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [Header("宝箱設定")]
    [SerializeField] public bool isMainTreasure = false;

    [Header("HP")]
    [SerializeField] public int maxHP = 500;

    public int currentHP;

    private bool isDead = false;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        Debug.Log(
            $"<color=yellow>[宝箱] ダメージ:{damage}" +
            $" 残HP:{currentHP}/{maxHP}</color>"
        );

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"<color=red>[宝箱] {gameObject.name} が破壊されました</color>");

        // この宝箱が置かれているTileを取得
        PlaceableObject placeableObject = GetComponent<PlaceableObject>();

        if (placeableObject != null && placeableObject.Tile != null)
        {
            Tile tile = placeableObject.Tile;

            tile.Type = TileType.Floor;
            tile.IsWalkable = true;
            tile.PlacedObject = null;
        }

        // メイン宝箱ならゲームオーバー
        if (isMainTreasure)
        {
            Debug.Log("<color=red>=== メイン宝箱が破壊されました！ゲームオーバー！ ===</color>");

            GameManager.Instance.GameOver();
            return;
        }

        Destroy(gameObject);
    }
}
