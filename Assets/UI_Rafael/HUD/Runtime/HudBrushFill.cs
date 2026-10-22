using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rafael.Hud
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HudBrushFill : MaskableGraphic
    {
        [SerializeField] private Vector2[] points = new Vector2[0];
        [SerializeField, Range(0f, 1f)] private float amount = 1f;

        public float Amount
        {
            get => amount;
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(amount, value)) return;
                amount = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (points.Length < 3 || amount <= 0f) return;
            float minimum = 1f, maximum = 0f;
            foreach (Vector2 point in points)
            {
                minimum = Mathf.Min(minimum, point.x);
                maximum = Mathf.Max(maximum, point.x);
            }
            float edge = Mathf.Lerp(minimum, maximum, amount);
            var clipped = new List<Vector2>(points.Length + 2);
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 a = points[i], b = points[(i + 1) % points.Length];
                bool insideA = a.x <= edge, insideB = b.x <= edge;
                if (insideA) clipped.Add(a);
                if (insideA != insideB)
                    clipped.Add(Vector2.Lerp(a, b, (edge - a.x) / (b.x - a.x)));
            }
            Rect rect = rectTransform.rect;
            foreach (Vector2 point in clipped)
                mesh.AddVert(new Vector3(rect.xMin + point.x * rect.width,
                    rect.yMin + point.y * rect.height), color, Vector2.zero);
            for (int i = 1; i + 1 < clipped.Count; i++) mesh.AddTriangle(0, i, i + 1);
        }
    }
}
