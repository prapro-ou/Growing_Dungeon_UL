using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

// aiueo
public class TileSelector : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private BuildManager buildManager;

    [SerializeField]
    private HoverHPUI hoverHPUI;

    [SerializeField]
    private PreviewManager previewManager;

    private Highlightable currentHighlight;

    private Tile lastTile;

    private void Update()
    {
        // UIの上にマウスがある場合
        if (EventSystem.current.IsPointerOverGameObject())
        {
            previewManager.HidePreview();
            hoverHPUI.Hide();

            if (currentHighlight != null)
            {
                currentHighlight.UnHighlight();
                currentHighlight = null;
            }

            return;
        }

        if (buildManager.CurrentMode == BuildMode.Erase)
        {
            EraseUpdate();
        }
        else
        {
            BuildUpdate();
        }
    }

    private void BuildUpdate()
    {
        if (currentHighlight != null)
        {
            currentHighlight.UnHighlight();
            currentHighlight = null;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            previewManager.HidePreview();
            hoverHPUI.Hide();
            return;
        }

        Monster monster = hit.collider.GetComponentInParent<Monster>();

        if (monster != null)
        {
            hoverHPUI.ShowMonster(monster);
        }
        else
        {
            Treasure treasure = hit.collider.GetComponentInParent<Treasure>();

            if (treasure != null)
            {
                hoverHPUI.ShowTreasure(treasure);
            }
            else
            {
                hoverHPUI.Hide();
            }
        }

        Tile tile = hit.collider.GetComponent<Tile>();

        if (tile == null)
        {
            previewManager.HidePreview();
            return;
        }

        if (tile.CanPlace(buildManager.CurrentMode))
        {
            previewManager.MovePreview(tile.transform.position);

            bool canPlace = buildManager.CanPlacePreview(tile);

            previewManager.SetPreviewValid(canPlace);
        }
        else
        {
            previewManager.HidePreview();
        }

        bool canDrag =
            buildManager.CurrentMode == BuildMode.Wall;

        if (canDrag)
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                lastTile = null;
                return;
            }
        }
        else
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;
        }

        if (tile == lastTile)
            return;

        lastTile = tile;

        buildManager.OnTileClicked(tile);
    }

    private void EraseUpdate()
    {
        previewManager.HidePreview();

        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            hoverHPUI.Hide();

            if (currentHighlight != null)
            {
                currentHighlight.UnHighlight();
                currentHighlight = null;
            }

            return;
        }

        // =========================
        // HP表示
        // =========================

        Monster monster = hit.collider.GetComponentInParent<Monster>();

        if (monster != null)
        {
            hoverHPUI.ShowMonster(monster);
        }
        else
        {
            Treasure treasure = hit.collider.GetComponentInParent<Treasure>();

            if (treasure != null)
            {
                hoverHPUI.ShowTreasure(treasure);
            }
            else
            {
                hoverHPUI.Hide();
            }
        }

        // =========================
        // 削除対象のハイライト
        // =========================

        PlaceableObject placeable =
            hit.collider.GetComponentInParent<PlaceableObject>();

        Highlightable highlight = null;

        if (placeable != null && placeable.Tile != null)
        {
            if (placeable.Tile.Type != TileType.Treasure)
            {
                highlight =
                    hit.collider.GetComponentInParent<Highlightable>();
            }
        }

        if (highlight != currentHighlight)
        {
            if (currentHighlight != null)
                currentHighlight.UnHighlight();

            currentHighlight = highlight;

            if (currentHighlight != null)
                currentHighlight.Highlight();
        }


        // =========================
        // 削除処理
        // =========================

        if (!Mouse.current.leftButton.isPressed)
        {
            lastTile = null;
            return;
        }

        if (placeable == null)
            return;

        if (placeable.Tile == lastTile)
            return;

        lastTile = placeable.Tile;

        buildManager.OnTileClicked(placeable.Tile);
    }
}