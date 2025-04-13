using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [Range(0f, 2f)]
    [SerializeField] float screenShakeIntensity = 1f;

    private Vector3 initialLocalPosition;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        initialLocalPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude = 0.2f)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude * screenShakeIntensity;
            float y = Random.Range(-1f, 1f) * magnitude * screenShakeIntensity;

            transform.localPosition = initialLocalPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        shakeRoutine = null;
    }
}
