using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;

    public GameObject[] characterIcons;

    [SerializeField] public Display[] display;

    private GameObject[] lastHovered;

    private void Awake()
    {
        instance = this;

        lastHovered = new GameObject[display.Length];
    }

    public void Inst(int playerIndex)
    {
        display[playerIndex].displayBox.gameObject.SetActive(true);
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
        if (playerIndex < 0 || playerIndex >= display.Length) return;
        if (hovered == lastHovered[playerIndex] || hovered == null) return;

        lastHovered[playerIndex] = hovered;

        var dis = hovered.GetComponent<CharacterIcon>();
        if (dis)
        {
            display[playerIndex].nameText.text = dis.characterData.characterName;
            display[playerIndex].titleText.text = dis.characterData.description;
            for (int i = 0; i < dis.characterData.abilityDescriptions.Length; i++)
            {
                display[playerIndex].abilityText[i].text = 
                    dis.characterData.abilityDescriptions[i].abilityName + "\n" +
                    dis.characterData.abilityDescriptions[i].abilityDescription;
            }
        }
    }

    public void SelectCharacter(SelectorController selector, GameObject selected, InputDevice input)
    {
        int index = selector.GetComponent<PlayerInput>().playerIndex;
        var data = selected.GetComponent<CharacterIcon>().characterData;
        display[index].bg.color = Color.black;

        PlayerStatManager.instance.SetCharacter(index, data, input);

        Debug.Log($"Player {index} selected: {data.characterName}");
    }

    public void DeselectCharacter(SelectorController selector)
    {
        int index = selector.GetComponent<PlayerInput>().playerIndex;
        display[index].bg.color = Color.gray;

        PlayerStatManager.instance.UnsetCharacter(index);

        display[index].displayBox.gameObject.SetActive(false);
    }
}

[Serializable]
public class Display
{
    public GameObject displayBox;
    public Image bg;
    public TMP_Text nameText;
    public TMP_Text titleText;
    public TMP_Text[] abilityText;
}