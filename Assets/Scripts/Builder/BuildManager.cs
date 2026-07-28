using UnityEngine;
using UnityEngine.InputSystem;

// aiueo
public class BuildManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject monsterPrefab;

    [SerializeField]
    private PreviewManager previewManager;

    public BuildMode CurrentMode = BuildMode.None;

    public void Start()
    {
        SetBuildMode(BuildMode.Wall);
    }

    public void OnTileClicked(Tile tile)
    {
        switch (CurrentMode)
        {
            case BuildMode.Wall:
                PlaceWall(tile);
                break;

            case BuildMode.Monster:
                PlaceMonster(tile);
                break;

            case BuildMode.Erase:
                EraseObject(tile);
                break;
        }
    }

    private void PlaceWall(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Wall))
            return;
        
        tile.Type = TileType.Wall;
        tile.IsWalkable = false;

        Vector3 position = tile.transform.position;

        GameObject wall = Instantiate(
            wallPrefab,
            position,
            Quaternion.identity,
            transform
        );

        wall.GetComponent<PlaceableObject>().Initialize(tile);

        tile.PlacedObject = wall;
    }

    private void PlaceMonster(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Monster))
            return;
        
        tile.Type = TileType.Monster;

        Vector3 position = tile.transform.position;

        GameObject monster = Instantiate(
            monsterPrefab,
            position,
            monsterPrefab.transform.rotation,
            transform
        );

        monster.GetComponent<PlaceableObject>().Initialize(tile);

        tile.PlacedObject = monster;
    }

    private void EraseObject(Tile tile)
    {
        if (tile.Type == TileType.Floor)
            return;
        
        if (tile.PlacedObject != null)
        {
            Destroy(tile.PlacedObject);
            tile.PlacedObject = null;
        }

        tile.Type = TileType.Floor;
        tile.IsWalkable = true;
    }

    public void SetBuildMode(BuildMode mode)
    {
        CurrentMode = mode;

        previewManager.SetPreview(mode);
    }
}