using UnityEngine;

public class MedKit : Interactable
{
    [SerializeField]
    private int amountToHeal;

    public override void TryInteract(Character entity = null)
    {
        if (entity == null)
        {
            Debug.LogError("No character sent to Health Kit.");
            return;
        }
        else
        {
            Interact(entity);
        }
    }

    protected override void Interact(Character entity = null)
    {
        // Search the character ValueBases for something that can be healed
        foreach (var value in entity.ValueBases)
        {
            if (value is IHealable healable && healable.NeedsHealing())
            {
                healable.Heal(amountToHeal);
                Destroy(gameObject); // Consume the item
                return;
            }
        }
    }
}
