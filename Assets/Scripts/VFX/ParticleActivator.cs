using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleActivator : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;

    public void PlayParticles()
    {
        particles.ForEach(particle => particle.Play());
    }
}
