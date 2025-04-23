using UnityEngine;
using UnityEngine.U2D.Animation;

public class Crescent : Projectile
{
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 0f);
    [SerializeField] Transform[] sprite;
    private void FixedUpdate()
    {
        foreach (Transform t in sprite)
        {
            t.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }
}