using UnityEngine;

public class DebuffManager : MonoBehaviour
{
    private const int MAX_DEBUFFS = 10;

    private DebuffInstance[] activeDebuffs = new DebuffInstance[MAX_DEBUFFS];
    private int activeCount = 0;

    void Update()
    {
        for (int i = activeCount - 1; i >= 0; i--)
        {
            var debuff = activeDebuffs[i];
            debuff.Tick(Time.deltaTime);

            if (debuff.remainingDuration <= 0)
            {
                debuff.template.OnRemove(gameObject);
                RemoveDebuffAt(i);
            }
        }
    }


    public void ApplyDebuff(Debuff so, int stacksToAdd, float? customDuration = null)
    {
        float duration = customDuration ?? so.defaultDuration;

        for (int i = 0; i < activeCount; i++)
        {
            if (activeDebuffs[i].template == so)
            {
                var debuff = activeDebuffs[i];
                debuff.ChangeStacks(debuff.currentStacks + stacksToAdd);

                if (so.removesOneAtATime == false)
                    debuff.RefreshDuration(duration);

                return;
            }
        }

        // Add new instance
        if (activeCount < activeDebuffs.Length)
        {
            var newInstance = new DebuffInstance();
            newInstance.Initialize(so, stacksToAdd, duration, gameObject);
            activeDebuffs[activeCount++] = newInstance;
        }
    }


    public void RemoveDebuff(string debuffID)
    {
        for (int i = 0; i < activeCount; i++)
        {
            if (activeDebuffs[i].template != null && activeDebuffs[i].template.debuffID == debuffID)
            {
                RemoveDebuffAt(i);
                return;
            }
        }
    }

    private void RemoveDebuffAt(int index)
    {
        activeDebuffs[index] = activeDebuffs[activeCount - 1];
        activeDebuffs[activeCount - 1] = null;
        activeCount--;
    }

    public int GetStacks(string debuffID)
    {
        for (int i = 0; i < activeCount; i++)
            if (activeDebuffs[i].template != null && activeDebuffs[i].template.debuffID == debuffID)
                return activeDebuffs[i].currentStacks;
        return 0;
    }

}
