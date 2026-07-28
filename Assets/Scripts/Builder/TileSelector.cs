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
    private PreviewManager previewManager;

    private Highlightable currentHighlight;

    private Tile lastTile;

    private void Update()
    {
        // UIの上にマウスがある場合
        if (EventSystem.current.IsPointerOverGameObject())
        {
            previewManager.HidePreview();

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
            return;
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

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            if (currentHighlight != null)
            {
                currentHighlight.UnHighlight();
                currentHighlight = null;
            }

            return;
        }

        Highlightable highlight =
            hit.collider.GetComponentInParent<Highlightable>();

        if (highlight != currentHighlight)
        {
            if (currentHighlight != null)
                currentHighlight.UnHighlight();

            currentHighlight = highlight;

            if (currentHighlight != null)
                currentHighlight.Highlight();
        }

        if (!Mouse.current.leftButton.isPressed)
        {
            lastTile = null;
            return;
        }

        PlaceableObject placeable =
            hit.collider.GetComponentInParent<PlaceableObject>();

        if (placeable == null)
            return;

        if (placeable.Tile == lastTile)
            return;

        lastTile = placeable.Tile;

        buildManager.OnTileClicked(placeable.Tile);
    }
}