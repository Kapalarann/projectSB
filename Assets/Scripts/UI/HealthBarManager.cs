using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] public GameObject healthBarPrefab;
    [SerializeField] float healthBarTime;

    private RectTransform healthBarInstance;
    private Transform characterTransform;

    private Dictionary<Transform, RectTransform> healthBars = new Dictionary<Transform, RectTransform>();

    public void AddHealthBar(Transform character)
    {
        GameObject instance = Instantiate(healthBarPrefab, canvasRect);
        healthBars[character] = instance.GetComponent<RectTransform>();
        instance.GetComponent<Image>().enabled = false;
    }

    public void RemoveHealthBar(Transform character)
    {
        Destroy(healthBars[character].gameObject);
        healthBars.Remove(character);
    }

    public void UpdateHealth(Transform character, float currentHealth, float maxHealth)
    {   
        healthBars[character].GetComponent<Image>().enabled = true;
        healthBars[character].Find("Health Fill").localScale = new Vector3(currentHealth / maxHealth, 1f, 1f);
    }

    void Update()
    {
        foreach (var pair in healthBars)
        {
            Transform character = pair.Key;
            if (!healthBars[character].GetComponent<Image>().enabled) continue;
            RectTransform healthBarRect = pair.Value;

            if (character != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(character.position + Vector3.up * 2f); // Offset height

                Vector2 localPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out localPosition))
                {
                    healthBarRect.localPosition = localPosition;
                }
            }
        }
    }
}
