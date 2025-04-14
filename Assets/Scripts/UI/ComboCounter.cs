using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ComboCounterUI : MonoBehaviour
{
    public static ComboCounterUI instance;

    [Header("UI References")]
    public TextMeshProUGUI comboText;
    public RectTransform decayBarFill;

    [Header("Shake & Scale Settings")]
    public float shakeDuration = 0.2f;
    public float baseShakeIntensity = 5f;
    public float scaleUpFactor = 1.2f;
    public float scaleDuration = 0.15f;

    [Header("Decay Settings")]
    public float comboDecayTime = 3f;
    private float decayTimer;

    private int currentCombo = 0;
    private float shakeTimer = 0f;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private RectTransform rectTransform;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        if(instance != null && instance != this)  Destroy(this);
        instance = this;
    }

    void Start()
    {
        rectTransform = comboText.GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        decayTimer = comboDecayTime;
        UpdateText();
        UpdateDecayBar();
    }

    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float intensity = baseShakeIntensity + currentCombo * 0.5f;
            Vector2 shakeOffset = Random.insideUnitCircle * intensity;
            rectTransform.anchoredPosition = originalPos + (Vector3)shakeOffset;
        }
        else
        {
            rectTransform.anchoredPosition = originalPos;
        }

        if (currentCombo > 0)
        {
            decayTimer -= Time.deltaTime;
            if (decayTimer <= 0f)
            {
                ResetCombo();
            }
            UpdateDecayBar();
        }
    }

    public void IncreaseCombo()
    {
        if (decayTimer <= 0f)
        {
            currentCombo = 1;
        }
        else
        {
            currentCombo++;
        }

        decayTimer = comboDecayTime;
        UpdateText();
        StartShake();
        AnimateScale();
        UpdateDecayBar();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        decayTimer = 0f;
        UpdateText();
        UpdateDecayBar();
    }

    void UpdateText()
    {
        comboText.text = currentCombo > 0 ? $"Combo x{currentCombo}" : "";
    }

    void UpdateDecayBar()
    {
        float t = Mathf.Clamp01(decayTimer / comboDecayTime);

        Vector3 scale = decayBarFill.localScale;
        scale.x = t;
        decayBarFill.localScale = scale;

        decayBarFill.gameObject.SetActive(currentCombo > 0);
    }

    void StartShake()
    {
        shakeTimer = shakeDuration;
    }

    void AnimateScale()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleBounce());
    }

    IEnumerator ScaleBounce()
    {
        Vector3 targetScale = originalScale * scaleUpFactor;
        float timer = 0f;

        while (timer < scaleDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, timer / scaleDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = targetScale;
        timer = 0f;

        while (timer < scaleDuration)
        {
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, timer / scaleDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }
}
