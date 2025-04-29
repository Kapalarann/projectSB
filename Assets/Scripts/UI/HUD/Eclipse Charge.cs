using UnityEngine;
using UnityEngine.UI;

public class EclipseCharge : MonoBehaviour
{
    [Header("UI References")]
    public Image moonFill;

    public void UpdateMeter(int amount, int max)
    {
        float fillAmount = (float)amount / max;
        moonFill.fillAmount = fillAmount;
    }
}
