using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] public float energyCost = 0f;
    [SerializeField] public float energyGenerated = 0f;

    [Header("Cooldown")]
    [SerializeField] public float cooldownDuration = 0f;
    public float cooldownTimer;

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public AnimationReciever _receiver;

    private Energy energy;

    public void Start()
    {
        energy = GetComponent<Energy>();
    }

    public void GenerateEnergy()
    {
        energy.ChangeEnergy(energyGenerated);
    }

    public bool HasEnoughEnergy()
    {
        if (energy.EP > energyCost) return true;
        return false;
    }

    public void ConsumeEnergy() 
    {
        energy.ChangeEnergy(-energyCost);
    }
}