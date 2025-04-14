using UnityEngine;
using UnityEngine.InputSystem;

public class Block : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] public float perfectBlockWindow = 0.2f;
    [Range(0f, 1f)]
    [SerializeField] public float blockValue = 0.5f;

    private float blockStartTime;
    private bool isBlocking;

    [Header("References")]
    private Health health;
    [SerializeField]private Animator animator;
    private InputAction inputAction;

    void Awake()
    {
        health = GetComponent<Health>();
        inputAction = GetComponent<PlayerInput>().actions["Block"];
    }

    public void OnBlock()
    {
        bool held = inputAction.ReadValue<float>() > 0.5f;

        if (held && !isBlocking)
        {
            StartBlock();
        }
        else if (!held && isBlocking)
        {
            CancelBlock();
        }
    }

    public void StartBlock()
    {
        isBlocking = true;
        blockStartTime = Time.time;
        if (animator != null) animator.SetBool("isBlocking", true);
    }

    public void CancelBlock()
    {
        if (!isBlocking) return;
        isBlocking = false;
        if (animator != null) animator.SetBool("isBlocking", false);
    }

    public bool TryBlock(float incomingDamage, Vector3 attackerPos, out bool isPerfect, out float poiseReduction)
    {
        isPerfect = false;
        poiseReduction = blockValue;
        if (!isBlocking)
            return false;

        float timeHeld = Time.time - blockStartTime;

        if (timeHeld <= perfectBlockWindow)
        {
            // TODO: reflect projectile or damage attacker’s poise
            isPerfect = true;
            return true;
        }
        else
        {
            return true;
        }
    }
}
