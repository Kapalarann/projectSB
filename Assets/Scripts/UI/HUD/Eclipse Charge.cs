using UnityEngine;
using UnityEngine.UI;

public class EclipseCharge : MonoBehaviour
{
    [Header("UI References")]
    public Image moonFill;

    private void Start()
    {
        transform.SetParent(FindFirstObjectByType<Canvas>().transform, false);
    }

    public void UpdateMeter(int amount, int max)
    {
        float fillAmount = (float)amount / max;
        moonFill.fillAmount = fillAmount;
    }
}
