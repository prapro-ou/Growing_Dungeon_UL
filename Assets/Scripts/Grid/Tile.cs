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
}