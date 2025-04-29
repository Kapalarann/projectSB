using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] public float staminaCost = 0f;

    [Header("Cooldown")]
    [SerializeField] public float cooldownDuration = 0f;
    public float cooldownTimer;

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public AnimationReciever _receiver;

    private Health health;

    public void Start()
    {
        health = GetComponent<Health>();
    }

    public void ConsumeStamina() 
    {
        health.ApplyStaminaDamage(staminaCost);
    }
}