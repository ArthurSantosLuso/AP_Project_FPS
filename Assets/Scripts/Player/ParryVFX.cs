using UnityEngine;

public class ParryVFX : MonoBehaviour
{
    [Header("Weapon animation")]
    [SerializeField] private Animator animator;

    public void TriggerParryFirstPartVFX()
    {
        animator.SetTrigger("Parry");
    }

    public void TriggerParrySecondPartVFX()
    {
        animator.SetTrigger("Absorb");
    }
}
