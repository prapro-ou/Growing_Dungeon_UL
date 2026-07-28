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

    public bool CanPlaceWall()
    {
        return Type == TileType.Floor;
    }

    public bool CanPlace(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Wall:
                return CanPlaceWall();

            case BuildMode.Monster:
                return CanPlaceWall();

            case BuildMode.Trap:
                return CanPlaceWall();

            case BuildMode.Treasure:
                return CanPlaceWall();

            default:
                return false;
        }
    }
}