using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VFX;

public class PistolShootVFX : MonoBehaviour
{
    [Header("Weapon animation")]
    [SerializeField] private Animator animator;
    
    [Space]
    [Header("Particle system. (Smoke, fire, flash & sparks")]
    [SerializeField] private List<ParticleSystem> particles;
    [SerializeField] float smokeEmitionDuration = 0.08f;

    [Space]
    [Header("Smoke VFX")]
    [SerializeField] private VisualEffect smokeVFX;

    [Space]
    [Header("Gunshot flash light")]
    [SerializeField] private Light shotFlashLight;
    [SerializeField] float peakIntensity = 8f;
    [SerializeField] float duration = 0.08f;
    [SerializeField] AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    private Coroutine _flashRoutine;
    private Coroutine _smokeRoutine;
    

    public void TriggerVFX()
    {
        animator.SetTrigger("Shoot");
        foreach (var particle in particles)
        {
            particle.Play();
        }

        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(ActivateShotFlashLight());

        if (_smokeRoutine != null) StopCoroutine(_smokeRoutine);
        _smokeRoutine = StartCoroutine(ActivateSmokeVFX());
    }

    private IEnumerator ActivateShotFlashLight()
    {
        shotFlashLight.enabled = true;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / duration;
            shotFlashLight.intensity = peakIntensity * falloff.Evaluate(normalized);
            yield return null;
        }

        shotFlashLight.intensity = 0;
        shotFlashLight.enabled = false;
        _flashRoutine = null;
    }

    private IEnumerator ActivateSmokeVFX()
    {
        smokeVFX.Play();
        float t = 0f;

        while (t < smokeEmitionDuration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        smokeVFX.Stop();
        _smokeRoutine = null;
    }
}
