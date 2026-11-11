public class TargetDummy : Condition, IDamageable
{
    public bool CanDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Damage(float damageValue)
    {
        SetConclusion(true);
        gameObject.SetActive(false);
    }

    public void DamageNoStagger(float damageValue)
    {
        throw new System.NotImplementedException();
    }

    public bool HasBlood()
    {
        throw new System.NotImplementedException();
    }
}
