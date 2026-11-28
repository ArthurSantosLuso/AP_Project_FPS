using UnityEngine;

public class Cheats : PausableMonoBehaviour
{
    [Header("Health Cheats Config")]
    [SerializeField] private KeyCode invulnerabilityKey;
    [SerializeField] private KeyCode damageSelfKey;
    [SerializeField] private int damageSelfAmount;
    [Header("Bullets Cheat Config")]
    [SerializeField] private KeyCode addAmmoKey;
    [SerializeField] private BulletData fireBullet;
    [SerializeField] private BulletData waterBullet;
    [SerializeField] private BulletData iceBullet;

    private PlayerHealth _health;
    private RevolverShot _revolver;

    private void Start()
    {
        _health = GetComponent<PlayerHealth>();
        _revolver = GetComponentInChildren<RevolverShot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(addAmmoKey) && _revolver != null)
        {
            _revolver.AddAmmo(fireBullet, 6);
            _revolver.AddAmmo(waterBullet, 10);
            _revolver.AddAmmo(iceBullet, 6);
        }

        if (Input.GetKeyDown(invulnerabilityKey) && _health != null)
        {
            _health.ToggleInvulnerability();
        }

        if (Input.GetKeyDown(damageSelfKey) && _health != null)
        {
            _health.DamageNoStagger(damageSelfAmount);
        }
    }
}
