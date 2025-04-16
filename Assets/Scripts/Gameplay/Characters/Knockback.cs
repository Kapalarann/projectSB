using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Knockback : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float knockbackResistance = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 sourcePosition, float knockbackForce, float angleDegrees = 0f)
    {
        Vector3 flatDirection = (transform.position - sourcePosition);
        flatDirection.y = 0;
        flatDirection.Normalize();

        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector3 knockbackDirection = flatDirection * Mathf.Cos(angleRadians) + Vector3.up * Mathf.Sin(angleRadians);

        Vector3 vel = _rb.linearVelocity;
        vel.y = 0f;
        _rb.linearVelocity = vel;
        _rb.AddForce(knockbackDirection * knockbackForce * GlobalValues.instance.knockbackStrengthMod * ( 1 - knockbackResistance ), ForceMode.Impulse);
    }
}
