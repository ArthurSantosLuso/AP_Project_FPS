using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    //[SerializeField]
    [SerializeField] private Animator crosshairAnimator;
    //private UIHandler uiHandler;

    private Character character;
    private FirstPersonMovement _firstPersonMovement;
   
    //private Interactor interactor;

    // Aim
    [SerializeField] private AudioClip aimInAudio;
    [SerializeField] private AudioClip aimOutAudio;
    public bool AimHeld { get; private set; }
   

    public Vector2 MoveInput { get; private set; }
    public bool SprintHeld { get; private set; }


    public Action OnWeaponChange;

    private void Start()
    {
        // uiHandler = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIHandler>();
        character = GetComponent<Character>();
        _firstPersonMovement = GetComponent<FirstPersonMovement>();
        // interactor = GetComponent<Interactor>();
    }

    public void OnWeaponChangeInput(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            bool changed = character.ChangeToNextWeapon();
            if (changed) OnWeaponChange?.Invoke();
        }
    }

    public void TriggerAbilityZero(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            character.UseAbility(0);
        }
    }

    public void TriggerAbilityOne(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            character.UseAbility(1);
        }
    }

    public void TriggerAbilityTwo(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            character.UseAbility(2);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
            SprintHeld = true;
        else if (context.phase == InputActionPhase.Canceled)
            SprintHeld = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            _firstPersonMovement.UseDash();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //if (DialogueManager.Instance.IsDialoguePlaying)
            //{
            //    DialogueManager.Instance.RequestNextLine();
            //    return;
            //}

            //interactor.ExecuteInteraction();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!VerifyIfPlayerCanAct()) return;

        if (context.phase == InputActionPhase.Performed)
        {
            AimHeld = true;
            AudioManager.Instance.PlaySound(aimInAudio);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            AimHeld = false;
            AudioManager.Instance.PlaySound(aimOutAudio);
        }

        crosshairAnimator.SetBool("IsAiming", AimHeld);
    }

    public void OnSkipDialogue(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //if (DialogueManager.Instance.IsDialoguePlaying)
            //{
            //    DialogueManager.Instance.ExitDialogueMode();
            //    return;
            //}
        }
    }

    //// >>>>>>>>>>>>>>> Temporary <<<<<<<<<<<<<<<<<<
    //public void OnInfiniteStamina(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Performed)
    //    {
    //        stamina.InfiniteStamina();
    //        health.InfiniteHealth();
    //    }
    //}

    public void OnToggleConfigCanva(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Performed)
        //{
        //    uiHandler.ToggleConfig();
        //}
    }

    private bool VerifyIfPlayerCanAct() => true/*GameManager.Instance.CanPlayerAct*/;
}