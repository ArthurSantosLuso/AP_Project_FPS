using System.Runtime.CompilerServices;
using UnityEngine;

public class AimCrosshair : MonoBehaviour
{
    private Animator crosshairAnimator;

    [SerializeField] private AudioClip aimInAudio;
    [SerializeField] private AudioClip aimOutAudio;

    private void Start()
    {
        GameObject player = GameManager.Instance.PlayerLocalInstance;

        player.GetComponent<PlayerInput>().OnAimInput += Aim;

        crosshairAnimator = GetComponent<Animator>();
    }

    private void Aim(bool isAiming)
    {
        crosshairAnimator.SetBool("IsAiming", isAiming);
        if (isAiming)
            AudioManager.Instance.PlaySound(aimInAudio);
        else AudioManager.Instance.PlaySound(aimOutAudio);
    }
}
