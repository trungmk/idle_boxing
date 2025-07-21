using UnityEngine;
using System;

public class CharacterAttackComponent : CharacterComponentBase
{
    private Character _character;
    private bool _isInitialized;
    private bool _isAttacking;
    private float _lastAttackTime;
    private float _attackSpeedMultiplier = 1f;
    private float _damageMultiplier = 1f;
    private float _criticalChance = 0f;
    private float _criticalDamage = 1f;

    public bool IsAttacking => _isAttacking;
    public bool CanAttack => !_isAttacking && Time.time - _lastAttackTime >= GetAttackCooldown();

    public event Action<Character> OnAttackStarted;
    public event Action<Character, float, bool> OnDamageDealt;
    public event Action OnAttackCompleted;

    public override void Initialize(Character character)
    {
        _character = character;
        _isInitialized = true;
    }

    public void AttackTarget(Character target)
    {
        if (!CanAttack || target == null)
        {
            return;
        }

        _isAttacking = true;
        _lastAttackTime = Time.time;
        _character.ChangeState(CharacterStateType.Attack);
        OnAttackStarted?.Invoke(target);

        Invoke(nameof(DealDamageToTarget), 0.3f);
    }

    private void DealDamageToTarget()
    {
        // This would be called at the right moment in the attack animation
        // For now, we'll find the nearest enemy
        Character target = FindNearestEnemy();
        if (target != null)
        {
            float baseDamage = _character.Stats.AttackPower;
            float finalDamage = CalculateFinalDamage(baseDamage);
            bool isCritical = UnityEngine.Random.value <= _criticalChance;

            if (isCritical)
            {
                finalDamage *= _criticalDamage;
            }

            target.TakeDamage(finalDamage, _character);
            OnDamageDealt?.Invoke(target, finalDamage, isCritical);
        }

        CompleteAttack();
    }

    private void CompleteAttack()
    {
        _isAttacking = false;
        OnAttackCompleted?.Invoke();
    }

    private Character FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(_character.Transform.position, _character.Stats.RangeAttackPower);

        Character nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Character other = collider.GetComponent<Character>();
            if (other != null && _character.IsEnemyOf(other) && !other.IsDead)
            {
                float distance = Vector3.Distance(_character.Transform.position, other.Transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = other;
                }
            }
        }

        return nearestEnemy;
    }

    private float CalculateFinalDamage(float baseDamage)
    {
        return baseDamage * _damageMultiplier;
    }

    private float GetAttackCooldown()
    {
        return _character.Stats.AttackCooldown / _attackSpeedMultiplier;
    }

    public void SetAttackSpeedMultiplier(float multiplier)
    {
        _attackSpeedMultiplier = multiplier;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        _damageMultiplier = multiplier;
    }

    public void SetCriticalChance(float chance)
    {
        _criticalChance = Mathf.Clamp01(chance);
    }

    public void SetCriticalDamage(float damage)
    {
        _criticalDamage = Mathf.Max(1f, damage);
    }
}