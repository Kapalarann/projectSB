using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
    public CharacterData characterData;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI description;

    private void FixedUpdate()
    {
        characterName.text = characterData.name;
        description.text = characterData.description;
    }
}
