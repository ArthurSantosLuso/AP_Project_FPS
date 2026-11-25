using UnityEngine;

public class ParryAbility : Ability
{

    [Header("Parrying")]
    [SerializeField] private float parryCooldown = 2f;
    [SerializeField] private AudioClip parrySuccessAudio;
    [SerializeField] private ParryVFX parryVFX;

    [Header("Remove Later")]
    [SerializeField] private BulletData bulletToAddWhenParry;
    [SerializeField] private RevolverShot revolver;

    private float _parryTimer;
    private bool _canParry = true;

    private void Update()
    {
        if (GameState.IsPaused) return;

        if (!_canParry)
        {
            _parryTimer += Time.deltaTime;

            if (_parryTimer > parryCooldown)
            {
                _canParry = true;
                _parryTimer = 0f;
            }
        }
    }

    public override void Perform()
    {
        if (CanAttack())
        {
            parryVFX.TriggerParryFirstPartVFX();
            _canParry = false;
            PlayActionAudio();
        }
    }

    protected override bool CanAttack()
    {
        if (!_canParry) return false;
        else return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Parryable parryable = other.GetComponent<Parryable>();

        if (parryable != null)
        {
            // Parried something
            parryVFX.TriggerParrySecondPartVFX();

            // Absorve bullet logic...
            revolver.AddAmmo(bulletToAddWhenParry, 2);

            parryable.Parry();
            AudioManager.Instance.PlaySound(parrySuccessAudio);
        }
    }
}
