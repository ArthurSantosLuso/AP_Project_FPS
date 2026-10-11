using UnityEngine;

public class ParryVFX : MonoBehaviour
{
    [Header("Weapon animation")]
    [SerializeField] private Animator animator;

    public void TriggerVFX()
    {
        animator.SetTrigger("Parry");
    }
}
