using UnityEngine;

public class ParryAbility : Ability
{
    [SerializeField] private ParryVFX parryVFX;

    [Header("Parrying")]
    [SerializeField] private float parryCooldown = 2f;

    private float   _parryTimer;
    private bool    _canParry = true;

    private void Update()
    {
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
            parryVFX.TriggerVFX();
            _canParry = false;
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
            // Absorve bullet logic...

            parryable.Parry();
        }
    }
}
