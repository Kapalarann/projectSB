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

        // Convert angle from degrees to radians and calculate vertical component
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector3 knockbackDirection = Quaternion.AngleAxis(angleDegrees, Vector3.Cross(flatDirection, Vector3.up)) * flatDirection;

        _rb.AddForce(knockbackDirection * knockbackForce * ( 1 - knockbackResistance ), ForceMode.Impulse);
    }
}
