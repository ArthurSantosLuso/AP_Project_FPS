using UnityEngine;

public class PunchVFX : MonoBehaviour
{
    [Header("Weapon animation")]
    [SerializeField] private Animator animator;

    public void TriggerVFX()
    {
        animator.SetTrigger("Punch");
    }
}
