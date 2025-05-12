using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Animator _animator;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRadius = 1f;
    [SerializeField] private LayerMask interactableLayer;

    public void OnInteract()
    {
        if (_animator.GetBool("isStunned")) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);
        Interactable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }
}
