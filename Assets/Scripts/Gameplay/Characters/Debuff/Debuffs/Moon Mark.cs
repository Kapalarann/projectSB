using UnityEngine;

[CreateAssetMenu(menuName = "Debuff System/Moon Debuff")]
public class MoonMarkSO : Debuff
{
    public override void OnRemove(GameObject target, GameObject source)
    {
        TotalEclipse te = source.GetComponent<TotalEclipse>();
        if (te != null)
        {
            te.IncreaseStack(1);
        }
    }
}
