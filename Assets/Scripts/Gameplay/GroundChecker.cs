using UnityEngine;

public static class GroundChecker
{
    public static bool IsGrounded(GameObject obj, float checkDistance = 0.1f, LayerMask? groundLayer = null)
    {
        if (obj == null) return false;

        Vector3 origin = obj.transform.position;
        Vector3 direction = Vector3.down;

        int layer = groundLayer ?? LayerMask.GetMask("Floor");

        return Physics.Raycast(origin, direction, checkDistance, layer);
    }

    public static bool IsGrounded(Transform transform, float checkDistance = 0.1f)
    {
        return IsGrounded(transform.gameObject, checkDistance, 6);
    }
}
