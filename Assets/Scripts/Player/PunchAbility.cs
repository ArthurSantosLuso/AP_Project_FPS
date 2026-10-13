using UnityEngine;

public class PunchAbility : Ability
{
    [SerializeField] private PunchVFX punchVFX;

    public override void Perform()
    {
        punchVFX.TriggerVFX();
    }

    protected override bool CanAttack()
    {
        throw new System.NotImplementedException();
    }
}
