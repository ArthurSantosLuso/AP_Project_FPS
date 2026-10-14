using System.Collections.Generic;
using UnityEngine;

public class OnAwakeTriggerEffect : MonoBehaviour
{
    [Header("Particle system. (Smoke, fire, flash & sparks")]
    [SerializeField] private List<ParticleSystem> particles;

    private void Awake()
    {
        foreach (var particle in particles)
        {
            particle.Play();
        }
    }
}
