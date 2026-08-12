using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private ChangeMenuUI changeMenuUI;

    [Header("Manager")]
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private PreviewManager previewManager;
    [SerializeField] private GameManager gameManager; // GameManagerを参照

    [Header("Build Buttons")]
    [SerializeField] private Button wallButton;
    [SerializeField] private Button trapButton;
    [SerializeField] private Button monsterButton;
    [SerializeField] private Button treasureButton;
    [SerializeField] private Button eraseButton;
    [SerializeField] private Button readyButton;

    [Header("Ready Button Text")]
    [SerializeField] private TextMeshProUGUI readyButtonText;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.7f, 1f);
    [SerializeField] private Color readyColor = Color.green;

    private bool isReady = false;

    private void Start()
    {
        UpdateButtonColors();
        UpdateReadyButtonColor();
    }

    private void OnEnable()
    {
        UpdateButtonColors();
    }

    public void Wall()
    {
        if (isReady) return;
        buildManager.SetBuildMode(BuildMode.Wall);
        UpdateButtonColors();
    }

    public void Trap()
    {
        if (isReady) return;
        buildManager.SetBuildMode(BuildMode.Trap);
        UpdateButtonColors();
    }

    public void Monster()
    {
        if (isReady) return;
        buildManager.SetBuildMode(BuildMode.Monster);

        changeMenuUI.ShowMonsterMenu();
        UpdateButtonColors();
    }

    public void Treasure()
    {
        if (isReady) return;
        buildManager.SetBuildMode(BuildMode.Treasure);
        UpdateButtonColors();
    }

    public void Erase()
    {
        if (isReady) return;
        buildManager.SetBuildMode(BuildMode.Erase);
        UpdateButtonColors();
    }

    // ★ On Click() から呼び出すトグルメソッド ★
    public void ToggleReady()
    {
        isReady = !isReady;

        if (isReady)
        {
            // 準備完了：建築モード解除＆プレビュー消去
            if (buildManager != null) buildManager.SetBuildMode(BuildMode.None);
            if (previewManager != null) previewManager.ClearPreview();

            // ★ 戦闘フェーズに移行（NavMesh再構築、敵スポーン開始など）
            if (gameManager != null)
            {
                gameManager.SetBattlePhase();
            }
        }
        else
        {
            // キャンセル：準備フェーズに戻す（敵のスポーン停止な''ど）
            if (gameManager != null)
            {
                gameManager.SetPreparationPhase();
            }
        }

        UpdateButtonColors();
        UpdateReadyButtonColor();
        SetBuildButtonsInteractable(!isReady);
    }

    private void UpdateButtonColors()
    {
        SetButtonColor(wallButton, BuildMode.Wall);
        SetButtonColor(trapButton, BuildMode.Trap);
        SetButtonColor(monsterButton, BuildMode.Monster);
        SetButtonColor(treasureButton, BuildMode.Treasure);
        SetButtonColor(eraseButton, BuildMode.Erase);
    }

    private void SetButtonColor(Button button, BuildMode mode)
    {
        if (button == null) return;

        button.image.color =
            (buildManager != null && buildManager.CurrentMode == mode)
            ? selectedColor
            : normalColor;
    }

    private void UpdateReadyButtonColor()
    {
        if (readyButton == null) return;
        
        readyButton.image.color = isReady ? readyColor : normalColor;

        if (readyButtonText != null)
        {
            readyButtonText.text = isReady ? "CANCEL" : "READY";
        }
    }

    private void SetBuildButtonsInteractable(bool interactable)
    {
        if (wallButton != null) wallButton.interactable = interactable;
        if (trapButton != null) trapButton.interactable = interactable;
        if (monsterButton != null) monsterButton.interactable = interactable;
        if (treasureButton != null) treasureButton.interactable = interactable;
        if (eraseButton != null) eraseButton.interactable = interactable;
    }
}