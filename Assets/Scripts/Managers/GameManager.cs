using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GamePhase
    {
        Preparation, // 準備フェーズ
        Battle       // 戦闘フェーズ
    }

    [Header("フェーズ状態")]
    [SerializeField] public GamePhase currentPhase = GamePhase.Preparation;

    // フェーズが変わったときに通知
    public event Action<GamePhase> OnPhaseChange;

    [Header("参照")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private WaveManager waveManager; // ★ EnemySpawner から WaveManager に変更
    [SerializeField] private Button startBattleButton; // 準備完了ボタン本体

    private void Start()
    {
        // 起動時は準備フェーズ
        SetPreparationPhase();
    }

    /// <summary>
    /// UIボタン（準備完了）から呼び出す関数
    /// </summary>
    public void OnClickStartBattle()
    {
        // 準備フェーズの時だけ実行
        if (currentPhase == GamePhase.Preparation)
        {
            SetBattlePhase();
        }
    }

    /// <summary>
    /// 準備フェーズに設定
    /// </summary>
    public void SetPreparationPhase()
    {
        currentPhase = GamePhase.Preparation;

        if (waveManager != null)
        {
            waveManager.EnterPrepPhase();
        }

        OnPhaseChange?.Invoke(currentPhase);
    }

    /// <summary>
    /// 戦闘フェーズに設定
    /// </summary>
    public void SetBattlePhase()
    {
        currentPhase = GamePhase.Battle;

        OnPhaseChange?.Invoke(currentPhase);

        // 1. 壁の配置に合わせて NavMesh を再構築
        if (gridManager != null)
        {
            gridManager.RebuildNavMesh();
        }

        // 2. ウェーブを開始！
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }
}