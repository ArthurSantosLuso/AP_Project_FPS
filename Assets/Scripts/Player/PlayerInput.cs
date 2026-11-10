using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Character character;
    private FirstPersonMovement _firstPersonMovement;

    public bool AimHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public Vector2 MoveInput { get; private set; }

    public Action OnWeaponChange;
    public Action<bool> OnAimInput;

    private void Start()
    {
        character = GetComponent<Character>();
        _firstPersonMovement = GetComponent<FirstPersonMovement>();
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
            OnAimInput?.Invoke(true);
            AimHeld = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnAimInput?.Invoke(false);
            AimHeld = false;
        }
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

    public void OnToggleConfigCanva(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Performed)
        //{
        //    uiHandler.ToggleConfig();
        //}
    }

    // If editor or web, pause via Tab
    // If windows, pause via esc
    public void OnTriggerPause(InputAction.CallbackContext context)
    {
        // If presses Tab on windows build, don't trigger pause
        if (Application.platform == RuntimePlatform.WindowsPlayer && context.control == Keyboard.current.pKey) return;

        // If presses ESC on editor or web build, don't trigger pause
        if ((Application.platform == RuntimePlatform.WebGLPlayer || Application.isEditor)
            && context.control == Keyboard.current.escapeKey)
            return;

        if (context.phase == InputActionPhase.Performed)
        {
            bool currentState = GameState.IsPaused;
            GameState.SetPaused(!currentState);
        }
    }

    private bool VerifyIfPlayerCanAct() => true/*GameManager.Instance.CanPlayerAct*/;
}