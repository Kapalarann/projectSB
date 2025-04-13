using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private bool isHitStopping = false;
    private float originalTimeScale = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DoHitStop(float duration, float timeScale = 0f)
    {
        if (!isHitStopping)
        {
            StartCoroutine(HitStopRoutine(duration, timeScale));
        }
    }

    private IEnumerator HitStopRoutine(float duration, float stopTimeScale)
    {
        isHitStopping = true;
        originalTimeScale = Time.timeScale;

        Time.timeScale = stopTimeScale;
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }
}
