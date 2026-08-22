using UnityEditor.ShaderGraph;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [Header("宝箱設定")]
    [SerializeField] public bool isMainTreasure = false;

    [Header("HP")]
    [SerializeField] public int maxHP = 500;

    public int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log(
            $"<color=yellow>[宝箱] ダメージ:{damage}" +
            $"残HP:{currentHP}/{maxHP}</color>"
        );

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>[宝箱] {gameObject.name} が破壊されました</color>");

        // この宝箱が置かれているTileを取得
        PlaceableObject placeableObject = GetComponent<PlaceableObject>();

        if (placeableObject != null && placeableObject.Tile != null)
        {
            Tile tile = placeableObject.Tile;

            // Tileをもとに戻す
            tile.Type = TileType.Floor;
            tile.IsWalkable = true;
            tile.PlacedObject = null;
        }

        Destroy(gameObject);
    }
}
