using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log($"{gameObject.name} was interacted with, but no specific behavior is defined.");
    }
}
