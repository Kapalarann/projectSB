using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    public static GlobalValues instance { get; private set; }

    [Header("Knockback")]
    public float knockbackStrengthMod = 1f;
    public float knockbackAngle = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
