using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] public GameObject healthBarPrefab;
    [SerializeField] float yOffset = 2f;
    [SerializeField] float healthBarTime;

    private RectTransform healthBarInstance;
    private Transform characterTransform;

    private Dictionary<Transform, RectTransform> healthBars = new Dictionary<Transform, RectTransform>();

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
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(character.position + Vector3.up * yOffset);

                Vector2 localPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out localPosition))
                {
                    healthBarRect.localPosition = localPosition;
                }
            }
        }
    }
}
