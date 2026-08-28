using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
    public Color colorTop = new Color32(27, 11, 46, 255);    // Viola scuro spaziale
    public Color colorBottom = new Color32(7, 4, 13, 255);   // Nero profondo

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        int count = vh.currentVertCount;
        if (count == 0) return;

        UIVertex vertex = new UIVertex();
        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            // I vertici superiori (0 e 1 o 1 e 2 a seconda del quad) prendono il top color
            vertex.color = (vertex.position.y > 0) ? colorTop : colorBottom;
            vh.SetUIVertex(vertex, i);
        }
    }
}