using UnityEngine;

public class Warcry : Ability
{
    [Header("Sphere Settings")]
    public float maxRadius = 5f;
    public float expansionDuration = 1f;
    public float staminaEffectAmount = 2f;

    [Header("Ally and Enemy Tags")]
    public string allyTag = "Player";
    public string enemyTag = "Enemy";

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
        if (cooldownTimer > 0f)
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
        if (!HasEnoughEnergy()) return;

        ScreenShake.Instance.Shake(0.2f, 0.2f);

        ConsumeEnergy();
        hasAppliedEffect = false;
        isActive = true;
        currentTime = 0f;
        sphereCollider.transform.localScale = Vector3.zero;
        sphereCollider.SetActive(true);
    }

    private void ApplyEffects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.gameObject == this.gameObject) continue;

            Stamina stamina = hit.GetComponent<Stamina>();
            if (hit.CompareTag(enemyTag) && stamina != null)
            {
                stamina.ApplyStaminaDamage(staminaEffectAmount);
            }

            Energy energy = hit.GetComponent<Energy>();
            if (hit.CompareTag(allyTag) && energy != null)
            {
                energy.ChangeEnergy(staminaEffectAmount);
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
