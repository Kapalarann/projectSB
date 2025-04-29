using UnityEngine;

public class TotalEclipse : MonoBehaviour
{
    [Header("Stacks")]
    [SerializeField] public int eclipseStacks = 0;
    [SerializeField] private int maxStacks = 8;

    [Header("Total Eclipse")]
    [SerializeField] public float duration = 10f;
    [SerializeField] public float damageMultiplier = 2f;
    [HideInInspector] public bool isTransformed = false;
    private float remainingTime = 0f;

    [Header("References")]
    [SerializeField] private GameObject effects;

    private ParticleSystem particle; 
    private PlayerMovement movement;
    private ShadowPuller shadowPuller;
    private EclipseCharge eclipseUI;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        shadowPuller = GetComponent<ShadowPuller>();
        eclipseUI = FindFirstObjectByType<EclipseCharge>();

        effects.SetActive(false);
        particle = effects.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!isTransformed) return;

        if(remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            isTransformed = false;
            effects.SetActive(false);
        }
    }

    public void IncreaseStack(int stackAmount)
    {
        eclipseStacks += stackAmount;
        Mathf.Clamp(eclipseStacks, 0, maxStacks);
        if(eclipseUI != null) eclipseUI.UpdateMeter(eclipseStacks, maxStacks);
    }

    public void OnUtility()
    {
        if (eclipseStacks >= maxStacks)
        {
            isTransformed = true;
            remainingTime = duration;
            IncreaseStack(-maxStacks);
            shadowPuller.IncreaseDamage(0f, damageMultiplier, 0f, damageMultiplier, duration);
            effects.SetActive(true);
            particle.Play();
        }
    }
}
