using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkEffect : MonoBehaviour
{
    private ParticleSystem sparkParticles;

    void Awake()
    {
        sparkParticles = GetComponent<ParticleSystem>();
    }

    public void PlayEffect(Vector3 position) // attacker's position
    {
        var shape = sparkParticles.shape;
        Vector3 dir = position - transform.position;
        shape.rotation = Quaternion.LookRotation(dir).eulerAngles;

        sparkParticles.Play();
    }
}
