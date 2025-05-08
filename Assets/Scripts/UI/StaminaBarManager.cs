using System.Collections.Generic;
using UnityEngine;

public class StaminaBarManager : MonoBehaviour
{
    [SerializeField] public GameObject staminaBarPrefab;
    [SerializeField] float yOffset = 2f; // The vertical offset from the character
    [SerializeField] float staminaBarTime;
    [SerializeField] private GameObject notchPrefab;

    private RectTransform canvasRect;

    private Dictionary<Transform, RectTransform> staminaBars = new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        canvasRect = GetComponent<RectTransform>();  // The Canvas' RectTransform
    }

    public void AddStaminaBar(Transform character)
    {
        GameObject instance = Instantiate(staminaBarPrefab, canvasRect);
        staminaBars[character] = instance.GetComponent<RectTransform>();
        staminaBars[character].gameObject.SetActive(false);
    }

    public void RemoveStaminaBar(Transform character)
    {
        Destroy(staminaBars[character].gameObject);
        staminaBars.Remove(character);
    }

    public void UpdateStamina(Transform character, float currentStamina, float maxStamina)
    {
        staminaBars[character].gameObject.SetActive(true);
        staminaBars[character].Find("Stamina Fill").localScale = new Vector3(currentStamina / maxStamina, 1f, 1f);
    }

    void Update()
    {
        foreach (var pair in staminaBars)
        {
            if (!pair.Key.gameObject.activeInHierarchy) continue;

            Transform character = pair.Key;
            RectTransform staminaBarRect = pair.Value;

            if (character != null)
            {
                // Convert the character's world position to screen space
                Vector3 worldPos = character.position + Vector3.up * yOffset; // Offset position to above the character
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos); // Convert world position to screen position

                // Convert screen position to local position relative to the canvas
                Vector2 localPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPosition))
                {
                    staminaBarRect.localPosition = localPosition;
                }
            }
        }
    }

    public void AddNotchToStaminaBar(Transform character, float percentage)
    {
        if (percentage < 0f || percentage > 1f) return;

        RectTransform staminaBar = staminaBars[character];
        float width = staminaBar.rect.width;
        percentage -= 0.5f;

        GameObject notch = Instantiate(notchPrefab, staminaBar);
        RectTransform notchRect = notch.GetComponent<RectTransform>();

        notchRect.anchoredPosition = new Vector2(width * percentage, 0f);
        notchRect.localScale = Vector3.one;
    }

}
