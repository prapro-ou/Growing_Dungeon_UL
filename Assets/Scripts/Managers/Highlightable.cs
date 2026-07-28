using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Renderer objectRenderer;

    private Color originalColor;

    private bool isHighlighted;

    private void Awake()
    {
        objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    public void Highlight()
    {
        if (objectRenderer == null || isHighlighted)
            return;

        isHighlighted = true;

        Color color = Color.red;
        color.a = originalColor.a;

        objectRenderer.material.color = color;
    }

    public void UnHighlight()
    {
        if (objectRenderer == null || !isHighlighted)
            return;

        isHighlighted = false;

        objectRenderer.material.color = originalColor;
    }
}