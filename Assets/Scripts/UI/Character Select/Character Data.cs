using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Select/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite icon;
    [TextArea] public string description;
    public AbilityDescription[] abilityDescriptions;
    public GameObject characterPrefab;
    public bool locked = false;
}

[Serializable]
public class AbilityDescription
{
    public string abilityName;
    public Image abilityIcon;
    [TextArea] public string abilityDescription;
}