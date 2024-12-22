using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffects : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    [SerializeField] private Material whiteFlashMaterial;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void FlashWhite(float flashDuration)
    {
        StartCoroutine(FlashRoutine(flashDuration));
    }

    private IEnumerator FlashRoutine(float flashDuration)
    {
        spriteRenderer.material = whiteFlashMaterial;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.material = originalMaterial;
    }
}
