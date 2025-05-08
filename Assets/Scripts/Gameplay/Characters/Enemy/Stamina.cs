using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] public float maxSP;
    [SerializeField] public float SP;
    [SerializeField] public float staminaRegen;
    [SerializeField] public float stamineDelay;
    [SerializeField] public AnimationCurve staminaRegenCurve;
    [SerializeField] public float regenCurveDuration = 2f;

    [Header("Stun")]
    [SerializeField] public float stunDuration = 1f;
    [SerializeField] public float stunResMultiplier;
    [SerializeField] public float stunResPerPlayer;
    private float lastStaminaUseTime;
    private bool stunEnded = false;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] AnimationReciever _receiver;
    [SerializeField] SpriteEffects spriteEffects;

    private StaminaBarManager staminaBar;
    private Audio _audio;
    [System.NonSerialized] public bool isStunned = false;

    private void Awake()
    {
        SP = maxSP;
        lastStaminaUseTime = Time.time;
        staminaBar = FindFirstObjectByType<StaminaBarManager>();
        _audio = GetComponent<Audio>();

        if (staminaBar != null)
        {
            staminaBar.AddStaminaBar(transform);
            staminaBar.UpdateStamina(transform, SP, maxSP);
        }

        _receiver.StunEnd += StunEnd;
    }

    private void OnDestroy()
    {
        staminaBar.RemoveStaminaBar(transform);

        _receiver.StunEnd -= StunEnd;
    }

    public void ApplyStaminaDamage(float amount)
    {
        if (isStunned)
        {
            AirCollider();
            return;
        }

        SP -= amount;
        SP = Mathf.Clamp(SP, 0, maxSP);

        if (SP <= 0) Stun();

        if (staminaBar != null) staminaBar.UpdateStamina(transform, SP, maxSP);

        lastStaminaUseTime = Time.time;
    }

    private void RegenerateStamina()
    {
        if (!isStunned && Time.time - lastStaminaUseTime >= stamineDelay)
        {
            if (SP < maxSP)
            {
                float timeSinceRegenStarted = Time.time - (lastStaminaUseTime + stamineDelay);
                float normalizedTime = Mathf.Clamp01(timeSinceRegenStarted / regenCurveDuration);
                float regenMultiplier = staminaRegenCurve.Evaluate(normalizedTime);

                SP += staminaRegen * regenMultiplier * Time.fixedDeltaTime;
                SP = Mathf.Clamp(SP, 0, maxSP);

                if (staminaBar != null)
                    staminaBar.UpdateStamina(transform, SP, maxSP);
            }
        }
    }

    void Stun()
    {
        if (_audio != null) _audio.PlayStunSound();
        HitStop.Instance.DoHitStop(0.2f);
        ScreenShake.Instance.Shake(0.1f, 0.05f);
        spriteEffects.FlashWhite(stunDuration / 10);
        isStunned = true;

        _animator.SetBool("isStunned", true);
        _animator.SetFloat("stunDuration", 1 / stunDuration);
    }

    public void StunEnd(AnimationEvent animationEvent)
    {
        EndStun();
    }

    private void EndStun()
    {
        GroundCollider();
        stunEnded = false;
        isStunned = false;
        _animator.SetBool("isStunned", false);
        SP = maxSP;
        staminaBar.UpdateStamina(transform, SP, maxSP);
    }

    private void FixedUpdate()
    {
        if (GroundChecker.IsGrounded(gameObject))
        {
            if (stunEnded) EndStun();
        }

        RegenerateStamina();
    }

    public void RecoverStamina(float amount)
    {
        if (isStunned) EndStun();

        SP += amount;
        SP = Mathf.Clamp(SP, 0, maxSP);

        if (staminaBar != null) staminaBar.UpdateStamina(transform, SP, maxSP);
    }

    private void AirCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void GroundCollider()
    {
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
    }
}
