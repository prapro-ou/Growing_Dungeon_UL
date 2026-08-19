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
    public GamePhase currentPhase = GamePhase.Preparation;

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
        Debug.Log("【準備フェーズ】配置を完了させたら「準備完了」ボタンを押してください。");

        // 準備フェーズなのでボタンを表示（有効化）する
        if (startBattleButton != null)
        {
            startBattleButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 戦闘フェーズに設定
    /// </summary>
    public void SetBattlePhase()
    {
        currentPhase = GamePhase.Battle;
        Debug.Log("【戦闘フェーズ】敵の攻撃開始！");

        // 戦闘中はボタンを非表示にする
        if (startBattleButton != null)
        {
            startBattleButton.gameObject.SetActive(false);
        }

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