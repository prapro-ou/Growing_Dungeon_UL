using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private MonsterData monsterData;

    [Header("Mode and Type")]
    public BuildMode CurrentMode = BuildMode.None;
    public MonsterType CurrentMonsterType = MonsterType.None;
    public TreasureType CurrentTreasureType = TreasureType.None;

    // 元のマテリアルの色を保存
    private Dictionary<Renderer, Color> originalColors =
        new Dictionary<Renderer, Color>();

    public void Start()
    {
        SetBuildMode(BuildMode.None);
    }

    /// <summary>
    /// タイルがクリックされたときのBuildMode検出
    /// </summary>
    /// <param name="tile"></param>
    public void OnTileClicked(Tile tile)
    {
        // 初期設置フェーズ
        if (waveManager.currentPhase == WaveManager.GamePhase.InitialSetup)
        {
            switch (CurrentMode)
            {
                case BuildMode.Treasure:
                    PlaceTreasure(tile);
                    break;

                case BuildMode.Erase:
                    EraseTreasure(tile);
                    break;
            }

            return;
        }

        // 通常の建築フェーズ以外は設置できない
        if (waveManager.currentPhase != WaveManager.GamePhase.PrepPhase)
            return;

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

            case BuildMode.Erase:
                EraseObject(tile);
                break;

            // 通常の建築フェーズでは宝箱は置かない
            case BuildMode.Treasure:
                return;
        }
    }


    private void PlaceWall(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Wall))
            return;
        
        // Wallコンポーネントを取得
        Wall wallData = wallPrefab.GetComponent<Wall>();
        if (wallData == null)
        {
            Debug.LogError("WallコンポーネントがPrefabについていません");
            return;
        }

        // DPが足りるか確認
        if (!dungeonPointManager.CanSpendDP(wallData.BuildCost))
        {
            Debug.Log("DPが足りません");
            return;
        }

        // DPを消費
        dungeonPointManager.SpendDP(wallData.BuildCost);

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

        PlacedObjectInfo info = wall.GetComponent<PlacedObjectInfo>();

        if (info != null)
        {
            info.Initialize(
                waveManager.currentWaveIndex,
                wallData.BuildCost
            );
        }

        tile.PlacedObject = wall;
    }

    private void PlaceMonster(Tile tile)
    {
        if (!tile.CanPlace(BuildMode.Monster))
            return;

        // いま選択している MonsterType を MonsterData 用に変換する
        MonsterData.MonsterType targetType = GetMonsterTypeFromCurrent();

        // MonsterDataから直接ステータス（コスト等）を取得する
        MonsterData.MonsterStatus status = monsterData.GetStatus(targetType);

        // DPが足りるか確認
        if (!dungeonPointManager.CanSpendDP(status.buildCost))
        {
            Debug.Log("DPが足りません");
            return;
        }

        // DPを消費
        dungeonPointManager.SpendDP(status.buildCost);

        tile.Type = TileType.Monster;

        Vector3 position = tile.transform.position;

        GameObject monsterPrefab = GetCurrentMonsterPrefab();

        if (monsterPrefab == null)
            return;

        GameObject monster = Instantiate(
            monsterPrefab,
            position,
            monsterPrefab.transform.rotation,
            transform
        );

        monster.GetComponent<PlaceableObject>().Initialize(tile);

        monster.GetComponent<Monster>().SetTile(tile);

        PlacedObjectInfo info = monster.GetComponent<PlacedObjectInfo>();

        if (info != null)
        {
            info.Initialize(
                waveManager.currentWaveIndex,
                status.buildCost // MonsterDataのコストを渡す
            );
        }

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
        // 初期設置フェーズ以外では宝箱を設置できない
        if (waveManager.currentPhase != WaveManager.GamePhase.InitialSetup)
        {
            CurrentTreasureType = TreasureType.None;
            SetBuildMode(BuildMode.None);
            return;
        }

        if (!tile.CanPlace(BuildMode.Treasure))
            return;

        // 現在設置されている宝箱を取得
        Treasure[] treasures = FindObjectsByType<Treasure>(
            FindObjectsInactive.Exclude
        );

        int mainTreasureCount = 0;
        int subTreasureCount = 0;

        foreach (Treasure currentTreasure in treasures)
        {
            if (currentTreasure.isMainTreasure)
            {
                mainTreasureCount++;
            }
            else
            {
                subTreasureCount++;
            }
        }

        // メイン宝箱は1個まで
        if (CurrentTreasureType == TreasureType.MainTreasure)
        {
            if (mainTreasureCount >= 1)
            {
                Debug.Log("メイン宝箱は1個までです");
                return;
            }
        }

        // サブ宝箱は3個まで
        if (CurrentTreasureType == TreasureType.SubTreasure)
        {
            if (subTreasureCount >= 3)
            {
                Debug.Log("サブ宝箱は3個までです");
                return;
            }
        }

        Vector3 position = tile.transform.position;

        GameObject treasurePrefab = null;

        switch (CurrentTreasureType)
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

    private void EraseTreasure(Tile tile)
    {
        // 宝箱以外は削除しない
        if (tile.Type != TileType.Treasure)
            return;

        if (tile.PlacedObject == null)
            return;

        Destroy(tile.PlacedObject);

        tile.PlacedObject = null;
        tile.Type = TileType.Floor;
        tile.IsWalkable = true;

        Debug.Log("初期設置中の宝箱を削除しました");
    }

    private void EraseObject(Tile tile)
    {
        if (tile.Type == TileType.Floor)
            return;

        // 宝箱は削除しない
        if (tile.Type == TileType.Treasure)
            return;

        if (tile.PlacedObject != null)
        {
            PlacedObjectInfo info =
                tile.PlacedObject.GetComponent<PlacedObjectInfo>();

            // 今WAVEで設置したものだけDPを返す
            if (info != null &&
                info.IsPlacedThisWave(waveManager.currentWaveIndex))
            {
                dungeonPointManager.AddDP(info.BuildCost);

                Debug.Log(
                    $"{info.BuildCost}DPを返却しました"
                );
            }

            Destroy(tile.PlacedObject);
            tile.PlacedObject = null;
        }

        tile.Type = TileType.Floor;
        tile.IsWalkable = true;
    }

    public bool CanPlacePreview(Tile tile)
    {
        if (tile == null)
            return false;

        // =========================
        // 壁
        // =========================
        if (CurrentMode == BuildMode.Wall)
        {
            if (!tile.CanPlace(BuildMode.Wall))
                return false;

            Wall wallData = wallPrefab.GetComponent<Wall>();

            if (wallData == null)
                return false;

            return dungeonPointManager.CanSpendDP(wallData.BuildCost);
        }


        // =========================
        // モンスター
        // =========================
        if (CurrentMode == BuildMode.Monster)
        {
            if (!tile.CanPlace(BuildMode.Monster))
                return false;

            MonsterData.MonsterType targetType = GetMonsterTypeFromCurrent();

            // MonsterDataからコストを取得して判定
            MonsterData.MonsterStatus status = monsterData.GetStatus(targetType);

            return dungeonPointManager.CanSpendDP(status.buildCost);
        }


        // =========================
        // 宝箱
        // =========================
        if (CurrentMode == BuildMode.Treasure)
        {
            if (!tile.CanPlace(BuildMode.Treasure))
                return false;

            Treasure[] treasures = FindObjectsByType<Treasure>(
                FindObjectsInactive.Exclude
            );

            int mainTreasureCount = 0;
            int subTreasureCount = 0;

            foreach (Treasure treasure in treasures)
            {
                if (treasure.isMainTreasure)
                    mainTreasureCount++;
                else
                    subTreasureCount++;
            }

            if (CurrentTreasureType == TreasureType.MainTreasure)
            {
                return mainTreasureCount < 1;
            }

            if (CurrentTreasureType == TreasureType.SubTreasure)
            {
                return subTreasureCount < 3;
            }

            return false;
        }

        return false;
    }

    private GameObject GetCurrentMonsterPrefab()
    {
        switch (CurrentMonsterType)
        {
            case MonsterType.Spider:
                return spiderPrefab;

            case MonsterType.Goblin:
                return goblinPrefab;

            case MonsterType.Gargoyle:
                return gargoylePrefab;

            case MonsterType.Skeleton:
                return skeletonPrefab;

            case MonsterType.Daemon:
                return daemonPrefab;

            case MonsterType.Golem:
                return golemPrefab;

            default:
                return null;
        }
    }

    // BuildManager側の MonsterType を MonsterData.MonsterType に変換する補助メソッド
    private MonsterData.MonsterType GetMonsterTypeFromCurrent()
    {
        switch (CurrentMonsterType)
        {
            case MonsterType.Spider: return MonsterData.MonsterType.Spider;
            case MonsterType.Goblin: return MonsterData.MonsterType.Goblin;
            case MonsterType.Gargoyle: return MonsterData.MonsterType.Gargoyle;
            case MonsterType.Skeleton: return MonsterData.MonsterType.Skeleton;
            case MonsterType.Daemon: return MonsterData.MonsterType.Daemon;
            case MonsterType.Golem: return MonsterData.MonsterType.Golem;
            default: return MonsterData.MonsterType.Spider;
        }
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
        Debug.Log($"SetTreasureType呼び出し: {type}");

        // 初期設置フェーズ以外では宝箱を選択できない
        if (waveManager.currentPhase != WaveManager.GamePhase.InitialSetup)
        {
            CurrentTreasureType = TreasureType.None;
            SetBuildMode(BuildMode.None);
            return;
        }

        CurrentTreasureType = type;

        if (type == TreasureType.None)
        {
            SetBuildMode(BuildMode.None);
            return;
        }

        SetBuildMode(BuildMode.Treasure);
    }

    public void ClearBuildSelection()
    {
        CurrentMode = BuildMode.None;
        CurrentMonsterType = MonsterType.None;
        CurrentTreasureType = TreasureType.None;

        if (previewManager != null)
        {
            previewManager.ClearPreview();
        }
    }

    public void UpdatePreviousWaveObjects()
    {
        PlacedObjectInfo[] objects =
            FindObjectsByType<PlacedObjectInfo>(
                FindObjectsInactive.Exclude
            );

        foreach (PlacedObjectInfo info in objects)
        {
            if (info.PlacedWave < waveManager.currentWaveIndex)
            {
                // 前WAVE → 半透明
                SetObjectTransparency(info.gameObject, 0.7f);
            }
            else
            {
                // 今WAVE → 通常表示
                SetObjectTransparency(info.gameObject, 1f);
            }
        }
    }


    private void SetObjectTransparency(GameObject obj, float alpha)
    {
        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            Color color = material.color;
            color.a = alpha;

            material.color = color;
        }
    }

    // 侵略モード開始時など、全オブジェクトを通常色に戻す
    public void ResetAllObjectColors()
    {
        foreach (KeyValuePair<Renderer, Color> pair in originalColors)
        {
            if (pair.Key == null)
                continue;

            Material material = pair.Key.material;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", pair.Value);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", pair.Value);
            }
        }

        originalColors.Clear();
    }

    public void ResetObjectTransparency()
    {
        PlacedObjectInfo[] objects =
            FindObjectsByType<PlacedObjectInfo>(
                FindObjectsInactive.Exclude
            );

        foreach (PlacedObjectInfo info in objects)
        {
            SetObjectTransparency(info.gameObject, 1f);
        }
    }
}