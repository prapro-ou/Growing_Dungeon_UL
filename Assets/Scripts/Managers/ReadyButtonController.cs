using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyButtonController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("State Colors")]
    [SerializeField] private Color notReadyColor = Color.red;
    [SerializeField] private Color readyColor = Color.green;

    // 現在の準備状態（外部から参照可能）
    public bool IsReady { get; private set; } = false;

    private void Start()
    {
        // ボタンのクリックイベントを登録
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(ToggleReadyState);
        }

        // 初期表示の更新
        UpdateButtonUI();
    }

    /// <summary>
    /// 準備状態のオン/オフを切り替える
    /// </summary>
    public void ToggleReadyState()
    {
        IsReady = !IsReady;
        UpdateButtonUI();

        if (IsReady)
        {
            OnReady();
        }
        else
        {
            OnCancelReady();
        }
    }

    private void UpdateButtonUI()
    {
        if (buttonText != null)
        {
            buttonText.text = IsReady ? "準備完了！" : "準備する";
        }

        // ボタンの背景色を変更（Target Graphic が設定されている場合）
        if (readyButton != null && readyButton.targetGraphic != null)
        {
            readyButton.targetGraphic.color = IsReady ? readyColor : notReadyColor;
        }
    }

    private void OnReady()
    {
        Debug.Log("準備完了状態になりました！");
        // ここに準備完了時の処理（例: カメラの位置をリセット、ゲーム開始カウントダウン等）を書く
    }

    private void OnCancelReady()
    {
        Debug.Log("準備状態をキャンセルしました。");
        // ここにキャンセル時の処理を書く
    }
}