using UnityEngine;


// aiueo
public class PlaceableObject : MonoBehaviour
{
    public Tile Tile { get; private set; }

    public void Initialize(Tile tile)
    {
        Tile = tile;
    }
}