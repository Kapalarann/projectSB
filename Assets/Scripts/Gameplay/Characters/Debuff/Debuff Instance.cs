using UnityEngine;

[System.Serializable]
public class DebuffInstance
{
    public Debuff template;
    public int currentStacks;
    public float remainingDuration;

    public GameObject source;
    private GameObject target;
    private GameObject visualEffectInstance;

    public void Initialize(Debuff template, int stacks, float duration, GameObject target, GameObject source)
    {
        this.template = template;
        this.currentStacks = stacks;
        this.remainingDuration = duration;
        this.source = source;
        this.target = target;

        if (template.visualEffectPrefab != null)
        {
            visualEffectInstance = Object.Instantiate(template.visualEffectPrefab, target.transform);
        }

        template.OnApply(target, stacks, source);
    }

    public void Tick(float deltaTime)
    {
        remainingDuration -= deltaTime;
        template.OnTick(target, currentStacks);
    }

    public void RefreshDuration(float duration)
    {
        remainingDuration = duration;
    }

    public void ChangeStacks(int newStack)
    {
        int prev = currentStacks;
        currentStacks = Mathf.Clamp(newStack, 0, template.maxStacks);

        if (currentStacks == 0)
        {
            RemoveVisual();
            template.OnRemove(target, source);
        }
    }

    private void RemoveVisual()
    {
        if (visualEffectInstance != null)
        {
            Object.Destroy(visualEffectInstance);
        }
    }
}
