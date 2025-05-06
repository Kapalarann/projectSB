using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] public float maxEP;
    public float EP = 0f;

    private StaminaBarManager staminaBar;

    private void Awake()
    {
        staminaBar = FindFirstObjectByType<StaminaBarManager>();

        if (staminaBar != null)
        {
            staminaBar.AddStaminaBar(transform);
            staminaBar.UpdateStamina(transform, EP, maxEP);
        }
    }

    private void OnDestroy()
    {
        staminaBar.RemoveStaminaBar(transform);
    }

    public void ChangeEnergy(float amount)
    {
        EP += amount;
        EP = Mathf.Clamp(EP, 0, maxEP);

        if (staminaBar != null) staminaBar.UpdateStamina(transform, EP, maxEP);
    }
}
