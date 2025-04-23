using UnityEngine;

[System.Serializable]
public class DebuffInstance
{
    public Debuff template;
    public int currentStacks;
    public float remainingDuration;

    private GameObject target;
    private GameObject visualEffectInstance;

    public void Initialize(Debuff template, int stacks, float duration, GameObject target)
    {
        this.template = template;
        this.currentStacks = stacks;
        this.remainingDuration = duration;
        this.target = target;

        if (template.visualEffectPrefab != null)
        {
            visualEffectInstance = Object.Instantiate(template.visualEffectPrefab, target.transform);
        }

        template.OnApply(target, stacks);
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
            template.OnRemove(target);
        }
    }

    public void ForceRemove()
    {
        RemoveVisual();
        template.OnRemove(target);
    }

    private void RemoveVisual()
    {
        if (visualEffectInstance != null)
        {
            Object.Destroy(visualEffectInstance);
        }
    }
}
