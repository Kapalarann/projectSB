using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Animator _animator;
    [SerializeField] Animator _attackEffect;
    [SerializeField] Transform _attackPoint;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] AnimationReciever _receiver;

    [Header("Attack 1 stats")]
    [SerializeField] float _attackRadius = 1.5f;
    [SerializeField] float _damage = 1f;

    public void OnAttack()
    {
        _animator.SetTrigger("onAttack");
    }

    private void Awake()
    {
        _receiver.AttackFrame += DealDamage;
    }
    private void OnDestroy()
    {
        _receiver.AttackFrame -= DealDamage;
    }
    public void DealDamage(AnimationEvent animationEvent)
    {
        _attackEffect.Play("Main_Character_Attack1_Effect");

        Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackRadius, _targetLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == this.gameObject) continue;

            Debug.Log("Hit: " + hit.name);

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
            }
        }
    }
}
