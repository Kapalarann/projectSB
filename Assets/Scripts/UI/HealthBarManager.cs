using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] public GameObject healthBarPrefab;
    [SerializeField] float yOffset = 2f; // The vertical offset from the character
    [SerializeField] float healthBarTime;

    private RectTransform canvasRect;

    private Dictionary<Transform, RectTransform> healthBars = new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        canvasRect = GetComponent<RectTransform>();  // The Canvas' RectTransform
    }

    public void AddHealthBar(Transform character)
    {
        GameObject instance = Instantiate(healthBarPrefab, canvasRect);
        healthBars[character] = instance.GetComponent<RectTransform>();
        healthBars[character].gameObject.SetActive(false);
    }

    public void RemoveHealthBar(Transform character)
    {
        Destroy(healthBars[character].gameObject);
        healthBars.Remove(character);
    }

    public void UpdateHealth(Transform character, float currentHealth, float maxHealth)
    {
        healthBars[character].gameObject.SetActive(true);
        healthBars[character].Find("Health Fill").localScale = new Vector3(currentHealth / maxHealth, 1f, 1f);
    }

    void Update()
    {
        foreach (var pair in healthBars)
        {
            if (!pair.Key.gameObject.activeInHierarchy) continue;

            Transform character = pair.Key;
            RectTransform healthBarRect = pair.Value;

            if (character != null)
            {
                // Convert the character's world position to screen space
                Vector3 worldPos = character.position + Vector3.up * yOffset; // Offset position to above the character
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos); // Convert world position to screen position

                // Convert screen position to local position relative to the canvas
                Vector2 localPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPosition))
                {
                    healthBarRect.localPosition = localPosition;
                }
            }
        }
    }
}
