using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Renderer[] objectRenderers;

    // ハイライトする直前の色
    private Color[] highlightColors;

    private bool isHighlighted;

    private void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>(true);

        highlightColors = new Color[objectRenderers.Length];
    }

    public void Highlight()
    {
        if (isHighlighted)
            return;

        isHighlighted = true;

        // 現在の色を保存
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                highlightColors[i] = material.GetColor("_BaseColor");
            }
            else if (material.HasProperty("_Color"))
            {
                highlightColors[i] = material.GetColor("_Color");
            }
        }

        // 赤くする
        Color redColor = Color.red;

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                redColor.a = highlightColors[i].a;
                material.SetColor("_BaseColor", redColor);
            }
            else if (material.HasProperty("_Color"))
            {
                redColor.a = highlightColors[i].a;
                material.SetColor("_Color", redColor);
            }
        }
    }

    public void UnHighlight()
    {
        if (!isHighlighted)
            return;

        isHighlighted = false;

        // ハイライトする前の色に戻す
        for (int i = 0; i < objectRenderers.Length; i++)
        {
            Material material = objectRenderers[i].material;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", highlightColors[i]);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", highlightColors[i]);
            }
        }
    }
}