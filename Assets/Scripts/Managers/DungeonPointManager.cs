using System;
using UnityEngine;

public class DungeonPointManager : MonoBehaviour
{
    public static DungeonPointManager Instance { get; private set; }

    [Header("Dungeon Point")]
    [SerializeField] private int currentDP = 100;

    // 現在のDPを取得する
    public int CurrentDP => currentDP;

    // DPが変更されたときに通知する
    public event Action<int> OnDPChanged;

    private void Awake()
    {
        // 既に存在する場合は重複（二重生成）を防ぐために自分を破壊する
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        // シーンを切り替えてもこのオブジェクトを破壊しないようにする
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log($"{currentDP}DP 所持しています");
    }

    public void AddDP(int amount)
    {
        if (amount <= 0)
            return;

        currentDP += amount;

        Debug.Log($"{amount}DP 獲得しました");
        Debug.Log($"{currentDP}DP 所持しています");

        OnDPChanged?.Invoke(currentDP);
    }

    public bool SpendDP(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentDP < amount)
            return false;

        currentDP -= amount;

        Debug.Log($"{amount}DP 使用しました");
        Debug.Log($"{currentDP}DP 所持しています");

        OnDPChanged?.Invoke(currentDP);

        return true;
    }

    // DPが足りるか確認する関数
    public bool CanSpendDP(int amount)
    {
        return currentDP >= amount;
    }
}
