using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;

    public GameObject[] characterIcons;

    // Arrays to support multiple players
    public GameObject[] displayBox;
    public TMP_Text[] nameText;
    public TMP_Text[] descriptionText;

    // Track last hovered for each player
    private GameObject[] lastHovered;

    private void Awake()
    {
        instance = this;

        // Ensure tracking for all players (assuming 4 max, or match your array sizes)
        lastHovered = new GameObject[displayBox.Length];
    }

    public void Inst(int playerIndex)
    {
        displayBox[playerIndex].gameObject.SetActive(true);
    }

    public GameObject GetHovered(Vector2 screenPos)
    {
        foreach (GameObject icon in characterIcons)
        {
            RectTransform rect = icon.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos))
            {
                return icon;
            }
        }
        return null;
    }

    public void UpdateHover(GameObject hovered, int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= displayBox.Length) return;
        if (hovered == lastHovered[playerIndex] || hovered == null) return;

        lastHovered[playerIndex] = hovered;

        var display = hovered.GetComponent<CharacterIcon>();
        if (display)
        {
            nameText[playerIndex].text = display.characterData.characterName;
            descriptionText[playerIndex].text = display.characterData.description;
        }
    }

    public void SelectCharacter(SelectorController selector, GameObject selected, InputDevice input)
    {
        int index = selector.GetComponent<PlayerInput>().playerIndex;
        var data = selected.GetComponent<CharacterIcon>().characterData;

        PlayerStatManager.instance.SetCharacter(index, data, input);

        Debug.Log($"Player {index} selected: {data.characterName}");
    }
}
