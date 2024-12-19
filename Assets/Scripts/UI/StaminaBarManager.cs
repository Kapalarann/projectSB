using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarManager : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] public GameObject staminaBarPrefab;
    [SerializeField] float yOffset = 2f;
    [SerializeField] float staminaBarTime;

    private RectTransform staminaBarInstance;
    private Transform characterTransform;

    private Dictionary<Transform, RectTransform> staminaBars = new Dictionary<Transform, RectTransform>();

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
