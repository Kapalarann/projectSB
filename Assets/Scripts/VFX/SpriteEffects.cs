using System.Collections;
using UnityEngine;

public class SpriteEffects : MonoBehaviour
{
    [SerializeField] Color flashColor = Color.white;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FlashWhite(float flashDuration)
    {
        StartCoroutine(FlashRoutine(flashDuration));
    }

    private IEnumerator FlashRoutine(float flashDuration)
    {
        spriteRenderer.material.SetColor("_FlashColor", flashColor);

        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;

            spriteRenderer.material.SetFloat("_FlashAmount", 
                Mathf.Lerp(1f, 0f, (elapsedTime / flashDuration))
                );

            yield return null;
        }
    }
}
