using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rafael.Hud
{
    public sealed class HudGameplayView : MonoBehaviour
    {
        [SerializeField] private PlayerHealth health;
        [SerializeField] private FirstPersonMovement movement;
        [SerializeField] private ParryAbility parry;
        [SerializeField] private RevolverShot revolver;
        [SerializeField] private Image healthFill;
        [SerializeField] private HudBrushFill dashFill;
        [SerializeField] private HudBrushFill parryFill;
        [SerializeField] private TMP_Text parryStatus;
        [SerializeField] private TMP_Text healthNumber;
        [SerializeField] private TMP_Text ammoNumber;
        [SerializeField] private TMP_Text ammoType;
        [SerializeField] private TMP_Text ammoStatus;
        [SerializeField] private Image bullet;
        [SerializeField] private Image selectedRing;
        [SerializeField] private Image ammoElementMark;
        [SerializeField] private Sprite waterSilhouette;
        [SerializeField] private Sprite fireSilhouette;
        [SerializeField] private Sprite iceSilhouette;

        // cooldowns are private, read via reflection (see link.xml)
        private const BindingFlags PrivateFields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo DashTimer = typeof(FirstPersonMovement).GetField("_dashCooldownTimer", PrivateFields);
        private static readonly FieldInfo DashCooldown = typeof(FirstPersonMovement).GetField("dashCooldown", PrivateFields);
        private static readonly FieldInfo ParryTimer = typeof(ParryAbility).GetField("_parryTimer", PrivateFields);
        private static readonly FieldInfo ParryCooldown = typeof(ParryAbility).GetField("parryCooldown", PrivateFields);
        private static readonly FieldInfo CanParry = typeof(ParryAbility).GetField("_canParry", PrivateFields);
        private static readonly FieldInfo NextShotTime = typeof(RevolverShot).GetField("_nextFireTime", PrivateFields);
        private static readonly FieldInfo FireRate = typeof(RevolverShot).GetField("fireRate", PrivateFields);
        private Material bulletMaterial;

        private void Awake()
        {
            if (bullet != null && bullet.material != null)
            {
                bulletMaterial = new Material(bullet.material);
                bullet.material = bulletMaterial;
            }
        }

        private void LateUpdate()
        {
            if (health != null)
            {
                float ratio = health.MaxValue > 0f ? Mathf.Clamp01(health.CurrentValue / health.MaxValue) : 0f;
                healthFill.fillAmount = Mathf.Ceil(ratio * 5f) / 5f;
                if (healthNumber != null) healthNumber.text = Mathf.CeilToInt(ratio * 5f) + " / 5";
            }
            if (movement != null && DashTimer != null && DashCooldown != null)
            {
                float duration = (float)DashCooldown.GetValue(movement);
                float remaining = (float)DashTimer.GetValue(movement);
                dashFill.Amount = duration > 0f ? 1f - Mathf.Clamp01(remaining / duration) : 1f;
            }
            if (parry != null && CanParry != null && ParryTimer != null && ParryCooldown != null)
            {
                bool ready = (bool)CanParry.GetValue(parry);
                float duration = (float)ParryCooldown.GetValue(parry);
                parryFill.Amount = ready || duration <= 0f ? 1f : Mathf.Clamp01((float)ParryTimer.GetValue(parry) / duration);
                parryFill.color = ready ? new Color(0.68f, 0.53f, 1f) : new Color(0.48f, 0.38f, 1f);
                if (parryStatus != null)
                {
                    parryStatus.text = ready ? "READY" : "CHARGING";
                    parryStatus.color = ready ? Color.white : new Color(0.8f, 0.75f, 1f);
                }
            }
            if (revolver == null) return;
            BulletData element = revolver.GetBulletAt(0);
            int ammunition = revolver.GetAmmoAt(0);
            ammoNumber.text = ammunition.ToString("00");
            ammoNumber.color = Color.white;
            bool hasAmmo = element != null && ammunition > 0;
            float nextShot = NextShotTime != null ? (float)NextShotTime.GetValue(revolver) : 0f;
            float rate = FireRate != null ? (float)FireRate.GetValue(revolver) : 0f;
            float shotInterval = rate > 0f ? 1f / rate : 0f;
            bool shotReady = hasAmmo && Time.time >= nextShot;
            bullet.fillAmount = !hasAmmo ? 0f : shotReady || shotInterval <= 0f ? 1f :
                1f - Mathf.Clamp01((nextShot - Time.time) / shotInterval);
            if (ammoStatus != null)
            {
                ammoStatus.text = !hasAmmo ? "NO AMMO" : shotReady ? "READY" : "CHARGING";
                ammoStatus.color = Color.white;
            }
            Color tint = new Color(0.92f, 0.92f, 0.92f);
            string label = "EMPTY";
            Sprite silhouette = null;
            if (element != null)
            {
                string elementName = element.name.ToLowerInvariant();
                if (elementName.Contains("water")) { label = "WATER"; tint = new Color(0.04f, 0.65f, 1f); silhouette = waterSilhouette; }
                else if (elementName.Contains("fire")) { label = "FIRE"; tint = new Color(1f, 0.18f, 0.04f); silhouette = fireSilhouette; }
                else if (elementName.Contains("ice")) { label = "ICE"; tint = new Color(0.38f, 0.93f, 1f); silhouette = iceSilhouette; }
                else label = element.name.Replace("Bullet", "").ToUpperInvariant();
            }
            ammoType.text = label;
            ammoType.color = Color.Lerp(tint, Color.white, 0.4f);
            selectedRing.color = tint;
            if (ammoElementMark != null)
            {
                ammoElementMark.sprite = silhouette;
                ammoElementMark.enabled = silhouette != null;
            }
            if (bulletMaterial != null)
            {
                bulletMaterial.SetColor("_BodyColor", tint);
                bulletMaterial.SetColor("_ShadowColor", tint * 0.45f);
                bulletMaterial.SetColor("_EdgeColor", Color.Lerp(tint, Color.white, 0.7f));
            }
        }

        private void OnDestroy()
        {
            if (bulletMaterial != null) Destroy(bulletMaterial);
        }
    }
}
