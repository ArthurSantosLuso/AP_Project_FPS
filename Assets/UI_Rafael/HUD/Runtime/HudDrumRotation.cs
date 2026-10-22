using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Rafael.Hud
{
    public sealed class HudDrumRotation : MonoBehaviour
    {
        [SerializeField] private RevolverShot revolver;
        [SerializeField] private RectTransform artwork;
        [SerializeField] private Image[] chamberIcons = new Image[6];
        [SerializeField, Min(0.01f)] private float rotationDuration = 0.16f;

        private static readonly FieldInfo InvertScroll = typeof(RevolverCylinderUI)
            .GetField("invertScroll", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly BulletData[] previous = new BulletData[6];
        private readonly BulletData[] chambers = new BulletData[6];
        private readonly BulletData[] current = new BulletData[6];
        private RevolverCylinderUI cylinderUI;
        private BulletData selected;
        private Quaternion restingRotation;
        private float angle, startAngle, targetAngle, elapsed, duration;
        private bool listening;

        private void OnEnable()
        {
            if (artwork == null || revolver == null) return;
            cylinderUI = GetComponent<RevolverCylinderUI>();
            restingRotation = artwork.localRotation;
            angle = startAngle = targetAngle = 0f;
            duration = elapsed = rotationDuration;
            selected = null;
            System.Array.Clear(chambers, 0, chambers.Length);
            System.Array.Clear(previous, 0, previous.Length);
            RefreshInventory(false);
            revolver.OnCylinderChanged += OnCylinderChanged;
            listening = true;
        }

        private void Start() => RefreshInventory(false);

        private void OnDisable()
        {
            if (!listening) return;
            if (revolver != null) revolver.OnCylinderChanged -= OnCylinderChanged;
            if (artwork != null) artwork.localRotation = restingRotation;
            foreach (Image icon in chamberIcons)
                if (icon != null) { icon.sprite = null; icon.enabled = false; }
            listening = false;
        }

        private void OnCylinderChanged() => RefreshInventory(true);

        private void RefreshInventory(bool animate)
        {
            if (revolver == null || artwork == null) return;
            for (int i = 0; i < current.Length; i++) current[i] = revolver.GetBulletAt(i);
            // each element keeps its chamber
            for (int i = 0; i < chambers.Length; i++)
                if (chambers[i] != null && System.Array.IndexOf(current, chambers[i]) < 0) chambers[i] = null;
            foreach (BulletData data in current)
            {
                if (data == null || System.Array.IndexOf(chambers, data) >= 0) continue;
                int free = System.Array.IndexOf(chambers, null);
                if (free >= 0) chambers[free] = data;
            }
            for (int i = 0; i < chamberIcons.Length && i < chambers.Length; i++)
            {
                if (chamberIcons[i] == null) continue;
                chamberIcons[i].sprite = chambers[i] != null ? chambers[i].bulletIcon : null;
                chamberIcons[i].enabled = chamberIcons[i].sprite != null;
            }
            BulletData next = current[0];
            if (next != null && (next != selected || !animate))
            {
                float goal = System.Array.IndexOf(chambers, next) * (360f / chambers.Length);
                if (!animate || selected == null)
                {
                    angle = startAngle = targetAngle = goal;
                    elapsed = duration;
                    artwork.localRotation = restingRotation * Quaternion.Euler(0f, 0f, angle);
                }
                else
                {
                    int direction = SelectionDirection();
                    float delta = direction > 0 ? Mathf.Repeat(goal - targetAngle, 360f) :
                        -Mathf.Repeat(targetAngle - goal, 360f);
                    startAngle = angle;
                    targetAngle += delta;
                    duration = rotationDuration * Mathf.Clamp(Mathf.Abs(targetAngle - startAngle) / 60f, 1f, 2f);
                    elapsed = 0f;
                }
            }
            selected = next;
            System.Array.Copy(current, previous, current.Length);
        }

        private int SelectionDirection()
        {
            int count = 0;
            while (count < previous.Length && previous[count] != null) count++;
            bool forward = count > 1, backward = count > 1;
            for (int i = 0; i < previous.Length; i++)
            {
                if (i < count)
                {
                    forward &= current[i] == previous[(i + 1) % count];
                    backward &= current[i] == previous[(i + count - 1) % count];
                }
                else if (current[i] != null) forward = backward = false;
            }
            int direction = forward ? 1 : -1;
            if (forward && backward && !Mathf.Approximately(Input.mouseScrollDelta.y, 0f))
            {
                bool invert = cylinderUI != null && InvertScroll != null && (bool)InvertScroll.GetValue(cylinderUI);
                direction = ((Input.mouseScrollDelta.y < 0f) ^ invert) ? 1 : -1;
            }
            return direction;
        }

        private void Update()
        {
            if (artwork == null || elapsed >= duration) return;
            elapsed = Mathf.Min(elapsed + Time.unscaledDeltaTime, duration);
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            angle = Mathf.LerpUnclamped(startAngle, targetAngle, t);
            artwork.localRotation = restingRotation * Quaternion.Euler(0f, 0f, angle);
        }
    }
}
