using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    private ParticleSystem bloodParticles;

    void Awake()
    {
        bloodParticles = GetComponent<ParticleSystem>();
    }

    public void PlayEffect(Vector3 position) // attacker's position
    {
        var shape = bloodParticles.shape;
        Vector3 dir = transform.position - position;
        shape.rotation = Quaternion.LookRotation(dir).eulerAngles;

        bloodParticles.Play();
    }
}
