using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReciever : MonoBehaviour
{
    public event Action<AnimationEvent> AttackWarning;
    public event Action<AnimationEvent> AttackFrame;
    public event Action<AnimationEvent> AttackEnd;
    public event Action<AnimationEvent> DashEnd;
    public event Action<AnimationEvent> StunEnd;
    void OnAttackWarning(AnimationEvent animationEvent)
    {
        AttackWarning?.Invoke(animationEvent);
    }
    void OnAttackFrame(AnimationEvent animationEvent)
    {
        AttackFrame?.Invoke(animationEvent);
    }

    void OnAttackEnd(AnimationEvent animationEvent)
    {
        AttackEnd?.Invoke(animationEvent);
    }
    void OnDashEnd(AnimationEvent animationEvent)
    {
        DashEnd?.Invoke(animationEvent);
    }

    void OnStunEnd(AnimationEvent animationEvent)
    {
        StunEnd?.Invoke(animationEvent);
    }
}
