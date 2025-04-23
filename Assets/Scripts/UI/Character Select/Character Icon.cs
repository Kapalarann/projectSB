using UnityEngine;

public class CharacterIcon : MonoBehaviour
{
    [SerializeField] public SpriteRenderer icon;
    public CharacterData characterData;

    private void Start()
    {
        icon.sprite = characterData.icon;
    }
}
