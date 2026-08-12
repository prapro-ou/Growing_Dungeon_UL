using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Information")]
    public int X;
    public int Y;

    public TileType Type = TileType.Floor;

    public bool IsWalkable = true;

    public GameObject PlacedObject;

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public bool CanPlace(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Wall:
                return Type == TileType.Floor;

            case BuildMode.Monster:
                return Type == TileType.Floor;

            case BuildMode.Trap:
                return Type == TileType.Floor;

            case BuildMode.Treasure:
                return Type == TileType.Floor;

            default:
                return false;
        }
    }
}