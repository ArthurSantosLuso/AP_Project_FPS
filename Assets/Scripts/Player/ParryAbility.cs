using UnityEngine;

public class ParryAbility : Ability
{
    [SerializeField] private ParryVFX parryVFX;
    public override void Perform()
    {
        if (!CanAttack()) return;

        parryVFX.TriggerVFX();
    }

    protected override bool CanAttack()
    {
        return true;
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
