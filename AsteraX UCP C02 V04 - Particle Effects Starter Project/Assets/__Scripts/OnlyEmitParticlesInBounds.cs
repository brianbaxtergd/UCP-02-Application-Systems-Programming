using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class OnlyEmitParticlesInBounds : MonoBehaviour
{
    ParticleSystem.EmissionModule emitter;

    void Start()
    {
        emitter = GetComponent<ParticleSystem>().emission;
    }

    void LateUpdate()
    {
        if (ScreenBounds.OOB(transform.position))
        {
            emitter.enabled = false;
        }
        else
        {
            emitter.enabled = true;
        }
    }
}
