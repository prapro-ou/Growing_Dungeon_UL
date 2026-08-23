using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Renderer[] objectRenderers;
    private Color[] originalColors;

    private bool isHighlighted;

    private void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>(true);

        originalColors = new Color[objectRenderers.Length];

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                originalColors[i] = material.GetColor("_BaseColor");
            }
            else if (material.HasProperty("_Color"))
            {
                originalColors[i] = material.GetColor("_Color");
            }
        }
    }

    public void Highlight()
    {
        if (isHighlighted)
            return;

        isHighlighted = true;

        Color color = Color.red;

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                color.a = originalColors[i].a;
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))
            {
                color.a = originalColors[i].a;
                material.SetColor("_Color", color);
            }
        }
    }

    public void UnHighlight()
    {
        if (!isHighlighted)
            return;

        isHighlighted = false;

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", originalColors[i]);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", originalColors[i]);
            }
        }
    }
}