using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

// aiueo
public class BuildManager : MonoBehaviour
{
    [Header("wallPrefab")]
    [SerializeField] private GameObject wallPrefab;

    [Header("monsterPrefab")]
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject gargoylePrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject daemonPrefab;
    [SerializeField] private GameObject golemPrefab;

    [Header("trapPrefab")]
    [SerializeField] private GameObject trapPrefab;

    [Header("treasurePrefab")]
    [SerializeField] private GameObject mainTreasurePrefab;
    [SerializeField] private GameObject subTreasurePrefab;

    [Header("参照")]
    [SerializeField] private PreviewManager previewManager;
    [SerializeField] private DungeonPointManager dungeonPointManager;

    [Header("Mode and Type")]
    public BuildMode CurrentMode = BuildMode.None;
    public MonsterType CurrentMonsterType = MonsterType.None;
    public TreasureType CurrentTreasureType = TreasureType.None;

    public void Start()
    {
        SetBuildMode(BuildMode.None);
    }

    /// <summary>
    ///  タイルがクリックされたときのBuildMode検出
    /// </summary>
    /// <param name="tile"></param>
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

            case BuildMode.Trap:
                PlaceTrap(tile);
                break;

            case BuildMode.Treasure:
                PlaceTreasure(tile);
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

        Vector3 position = tile.transform.position;

        GameObject monsterPrefab = null;

        // MonsterTypeにあわせたPrefabに変更
        switch(CurrentMonsterType)
        {
            case MonsterType.Spider:
                monsterPrefab = spiderPrefab;
                break;

            case MonsterType.Goblin:
                monsterPrefab = goblinPrefab;
                break;

            case MonsterType.Gargoyle:
                monsterPrefab = gargoylePrefab;
                break;

            case MonsterType.Skeleton:
                monsterPrefab = skeletonPrefab;
                break;

            case MonsterType.Daemon:
                monsterPrefab = daemonPrefab;
                break;

            case MonsterType.Golem:
                monsterPrefab = golemPrefab;
                break;
        }

        if (monsterPrefab == null)
            return;

        // Monsterコンポーネントを取得
        Monster monsterData = monsterPrefab.GetComponent<Monster>();
        if (monsterData == null)
        {
            Debug.LogError("MonsterコンポーネントがPrefabについていません");
            return;
        }

        // DPが足りるか確認
        if (!dungeonPointManager.CanSpendDP(monsterData.BuildCost))
        {
            Debug.Log("DPが足りません");
            return;
        }

        // DPを消費
        dungeonPointManager.SpendDP(monsterData.BuildCost);

        tile.Type = TileType.Monster;

        GameObject monster = Instantiate(
            monsterPrefab,
            position,
            monsterPrefab.transform.rotation,
            transform
        );

        monster.GetComponent<PlaceableObject>().Initialize(tile);

        tile.PlacedObject = monster;
    }

    private void PlaceTrap(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Trap))
            return;
        
        Vector3 position = tile.transform.position;

        tile.Type = TileType.Trap;

        GameObject trap = Instantiate(
            trapPrefab,
            position,
            trapPrefab.transform.rotation,
            transform
        );

        trap.GetComponent<PlaceableObject>().Initialize(tile);

        tile.PlacedObject = trap;
    }

    private void PlaceTreasure(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Treasure))
            return;

        Vector3 position = tile.transform.position;

        GameObject treasurePrefab = null;

        // TresureTypeにあわせたPrefabに変更
        switch(CurrentTreasureType)
        {
            case TreasureType.MainTreasure:
                treasurePrefab = mainTreasurePrefab;
                break;

            case TreasureType.SubTreasure:
                treasurePrefab = subTreasurePrefab;
                break;
        }

        if (treasurePrefab == null)
            return;

        tile.Type = TileType.Treasure;

        GameObject treasure = Instantiate(
            treasurePrefab,
            position,
            treasurePrefab.transform.rotation,
            transform
        );

        treasure.GetComponent<PlaceableObject>().Initialize(tile);

        tile.PlacedObject = treasure;  
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

    public void SetMonsterType(MonsterType type)
    {
        CurrentMonsterType = type;

        if (type == MonsterType.None)
        {
            SetBuildMode(BuildMode.None);
            return;
        }

        SetBuildMode(BuildMode.Monster);
    }

    public void SetTreasureType(TreasureType type)
    {
        CurrentTreasureType = type;

        if (type == TreasureType.None)
        {
            SetBuildMode(BuildMode.None);
            return;
        }

        SetBuildMode(BuildMode.Treasure);
    }
}