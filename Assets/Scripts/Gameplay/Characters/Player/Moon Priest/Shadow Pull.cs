using UnityEngine;

public class ShadowPull : MonoBehaviour
{
    [Header("Shadow Pull Settings")]
    public Transform shadowTransform; // Shadow quad/mesh
    public AnimationCurve stretchOverTime;

    public float timer = 0f;
    private float pullDuration = 1f;
    private float maxStretch = 2f;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 pullDirection = Vector3.up; // Default moonward

    private bool isShadowActive = false;

    [Header("Spectral Projection Settings")]
    public GameObject spectralClone;
    public Vector3 targetPosition;
    public AnimationCurve moveOverTime;
    public float pullDistance = 3f;
    public float fadeDuration = 2.0f;
    public float scaleIncrease = 0.2f;

    public Vector3 offset;
    private bool isSpectralActive = false;
    private float spectralTimer = 0f;
    private Vector3 initialScale;

    void Start()
    {
        originalScale = shadowTransform.localScale;
        originalRotation = shadowTransform.localRotation;

        if (spectralClone != null)
        {
            spectralClone.SetActive(false); // Deactivate the spectral clone by default
        }

        offset = spectralClone.transform.localPosition;
        initialScale = spectralClone.transform.localScale;
    }

    void FixedUpdate()
    {
        // Handle the Shadow Pull Logic
        HandleShadowPull();

        // Handle the Spectral Projection Logic
        HandleSpectralProjection();
    }

    private void HandleShadowPull()
    {
        if (!isShadowActive) return;

        timer += Time.fixedDeltaTime;
        float t = timer / pullDuration;

        if (t >= 1f)
        {
            ResetShadow();
        }
        else
        {
            float stretchAmount = stretchOverTime.Evaluate(t) * maxStretch;
            shadowTransform.localScale = new Vector3(originalScale.x, originalScale.y + stretchAmount, originalScale.z);

            if (pullDirection != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(pullDirection.normalized, Vector3.up);
                shadowTransform.rotation = lookRot;
            }
        }
    }

    private void HandleSpectralProjection()
    {
        if (!isSpectralActive || spectralClone == null) return;

        spectralTimer += Time.fixedDeltaTime;
        float t = spectralTimer / pullDuration;

        if (t >= 1f)
        {
            spectralClone.SetActive(false);
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distanceTraveled = moveOverTime.Evaluate(t) * pullDistance;
            spectralClone.transform.position = transform.position + direction * distanceTraveled + offset;

            float curveValue = moveOverTime.Evaluate(t);
            spectralClone.transform.localScale = initialScale * (1 + ( scaleIncrease * curveValue));

            HandleSpectralFadeEffect();
        }
    }

    public void PullTowards(Vector3 worldPosition, float pullStrength, float pullDuration)
    {
        // Shadow Pull
        pullDirection = (shadowTransform.position - worldPosition).normalized;
        maxStretch = pullStrength;
        pullDistance = pullStrength / 10f;
        this.pullDuration = pullDuration;
        timer = 0f;
        isShadowActive = true;

        // Spectral
        targetPosition = worldPosition;
        StartSpectralProjection();
    }

    private void StartSpectralProjection()
    {
        if (spectralClone != null && !spectralClone.activeSelf)
        {
            spectralClone.SetActive(true);  // Activate the spectral clone when needed
            initialScale = spectralClone.transform.localScale;
            spectralTimer = 0f;
            isSpectralActive = true;
        }
    }

    private void HandleSpectralFadeEffect()
    {
        float timerValue = Mathf.PingPong(Time.time / fadeDuration, 1);
        Color cloneColor = spectralClone.GetComponent<SpriteRenderer>().color;
        spectralClone.GetComponent<SpriteRenderer>().color = new Color(cloneColor.r, cloneColor.g, cloneColor.b, Mathf.Clamp01(timerValue));
    }

    private void ResetShadow()
    {
        isShadowActive = false;
        shadowTransform.localScale = originalScale;
        shadowTransform.localRotation = originalRotation;
    }
}
