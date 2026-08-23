using UnityEngine;

public class PlacedObjectInfo : MonoBehaviour
{
    // 何WAVEで設置されたか
    public int PlacedWave { get; private set; }

    // 設置時のコスト
    public int BuildCost { get; private set; }

    /// <summary>
    /// 設置時に呼び出す
    /// </summary>
    public void Initialize(int wave, int cost)
    {
        PlacedWave = wave;
        BuildCost = cost;
    }

    /// <summary>
    /// 今WAVEで設置されたものか
    /// </summary>
    public bool IsPlacedThisWave(int currentWave)
    {
        return PlacedWave == currentWave;
    }
}