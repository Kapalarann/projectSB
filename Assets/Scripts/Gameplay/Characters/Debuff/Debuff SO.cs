using UnityEngine;
public abstract class Debuff : ScriptableObject
{
    public string debuffID;
    public int maxStacks = 5;
    public float defaultDuration = 5f;
    public bool removesOneAtATime = false;
    public GameObject visualEffectPrefab;

    public virtual void OnTick(GameObject target, int currentStacks) { }
    public virtual void OnApply(GameObject target, int stacksAdded) { }
    public virtual void OnRemove(GameObject target) { }
}
