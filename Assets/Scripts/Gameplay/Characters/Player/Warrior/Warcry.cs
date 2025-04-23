using UnityEngine;

public class Warcry : MonoBehaviour
{
    [Header("Sphere Settings")]
    public float maxRadius = 5f;
    public float expansionDuration = 1f;
    public float staminaEffectAmount = 2f;

    [Header("Ally and Enemy Tags")]
    public string allyTag = "Player";
    public string enemyTag = "Enemy";

    [Header("Cooldown")]
    public float cooldownDuration = 5f;
    private float cooldownTimer = 0f;
    public bool IsOnCooldown => cooldownTimer > 0f;

    [Header("References")]
    public GameObject sphereCollider;
    private float currentTime = 0f;
    private bool hasAppliedEffect = false;
    private bool isActive = false;

    void Awake()
    {
        sphereCollider.transform.localScale = Vector3.zero;
        sphereCollider.SetActive(false); // start inactive by default
    }

    void Update()
    {
        if (IsOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isActive) return;

        currentTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentTime / expansionDuration);
        sphereCollider.transform.localScale = Vector3.one * Mathf.Lerp(0f, maxRadius, progress);

        if (progress >= 1f && !hasAppliedEffect)
        {
            hasAppliedEffect = true;
            ApplyEffects();
            EndEffect();
        }
    }

    public void OnSpecial()
    {
        if (IsOnCooldown) return;

        ScreenShake.Instance.Shake(0.2f, 0.2f);

        hasAppliedEffect = false;
        isActive = true;
        currentTime = 0f;
        sphereCollider.transform.localScale = Vector3.zero;
        sphereCollider.SetActive(true);

        cooldownTimer = cooldownDuration;
    }

    private void ApplyEffects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxRadius);

        foreach (var hit in hitColliders)
        {
            Health health = hit.GetComponent<Health>();
            if (health == null) continue;

            if (hit.CompareTag(enemyTag))
            {
                health.ApplyStaminaDamage(staminaEffectAmount);
            }
            else if (hit.CompareTag(allyTag))
            {
                health.RecoverStamina(staminaEffectAmount);
            }
        }
    }

    private void EndEffect()
    {
        sphereCollider.transform.localScale = Vector3.zero;
        currentTime = 0f;
        isActive = false;
        sphereCollider.SetActive(false);
    }
}
