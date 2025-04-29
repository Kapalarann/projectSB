using UnityEngine;

public class PlayerAttack : Ability
{
    [Header("Reference")]
    [SerializeField] Animator _attackEffect;
    [SerializeField] Transform _attackPoint;
    [SerializeField] LayerMask _targetLayer;

    [Header("Attack 1 stats")]
    [SerializeField] float _attackRadius = 1.5f;
    [SerializeField] float _damage = 1f;
    [SerializeField] float _attackSpeed = 1f;

    public void OnPrimary()
    {
        _animator.SetTrigger("onAttack");
        _animator.SetFloat("attackSpeed", _attackSpeed);
        _attackEffect.SetFloat("attackSpeed", _attackSpeed);
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
        gameObject.GetComponent<Audio>().PlaySlashSound();
        _attackEffect.Play("Main_Character_Attack1_Effect");

        Collider[] hitColliders = Physics.OverlapSphere(_attackPoint.position, _attackRadius, _targetLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == this.gameObject) continue;

            Health health = hit.GetComponent<Health>();
            if (health == null) return;
            if (health.isInvulnerable)
            {
                health.TakeDamage(0f, transform.position);
            }
            else
            {
                health.TakeDamage(_damage, transform.position);
            }
        }
        ConsumeStamina();
    }
}
