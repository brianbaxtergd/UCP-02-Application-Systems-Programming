using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipExhaustParticles : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject partGO;

    Rigidbody rigid;
    ParticleSystem partSys;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        partSys = partGO.GetComponent<ParticleSystem>();
        if (partSys == null)
            Debug.LogError("ShipExhaustParticles:partSys is null.");
    }

    void Update()
    {
        if (rigid.velocity != Vector3.zero)
        {
            // If the player ship is moving, update position of particles to follow ship.
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 5f);
            partGO.transform.position = newPos;
            // Update partGO orientation to match ship's velocity.
            partGO.transform.LookAt(rigid.position + rigid.velocity);
            // Turn particle system on.
            if (!partSys.isEmitting)
                partSys.Play();
        }
        else
        {
            // Turn particle system off.
            if (partSys.isPlaying)
                partSys.Stop();
        }
    }
}
