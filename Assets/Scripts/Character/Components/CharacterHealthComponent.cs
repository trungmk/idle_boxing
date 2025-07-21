using UnityEngine;
using System;

public class CharacterHealthComponent : CharacterComponentBase
{
    private Character _character;
    private bool _isInitialized;
    private float _currentHealth;
    private float _maxHealth;
    private bool _isDead;
    private bool _isStunned;
    private float _stunTimer;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float HealthPercentage => _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;
    public bool IsDead => _isDead;
    public bool IsStunned => _isStunned;

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action OnStunned;
    public event Action OnStunRecovered;

    public override void Initialize(Character character)
    {
        _character = character;
        _maxHealth = _character.Stats.MaxHealth;
        _currentHealth = _maxHealth;
        _isInitialized = true;
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (_isStunned)
        {
            _stunTimer -= deltaTime;
            if (_stunTimer <= 0f)
            {
                RecoverFromStun();
            }
        }
    }

    public void TakeDamage(float damage, Character attacker = null)
    {
        if (_isDead)
        {
            return;
        } 
            

        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            _character.ChangeState(CharacterStateType.Hit);
        }
    }

    public void Heal(float amount)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void ApplyStun(float duration)
    {
        if (_isDead) return;

        _isStunned = true;
        _stunTimer = duration;
        _character.ChangeState(CharacterStateType.Stunned);
        OnStunned?.Invoke();
    }

    private void RecoverFromStun()
    {
        _isStunned = false;
        _stunTimer = 0f;
        OnStunRecovered?.Invoke();

        if (_character.CurrentState == CharacterStateType.Stunned)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }

    private void Die()
    {
        _isDead = true;
        OnDeath?.Invoke();
    }

    public void Revive(float healthPercentage = 1f)
    {
        _isDead = false;
        _currentHealth = _maxHealth * healthPercentage;
        OnHealthChanged?.Invoke(_currentHealth);
        OnRevive?.Invoke();
    }
}