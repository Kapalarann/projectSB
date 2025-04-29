using UnityEngine;

public class Spectral : MonoBehaviour
{
    public Transform sprite;
    public SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        transform.localScale = sprite.localScale;
        GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
    }
}
