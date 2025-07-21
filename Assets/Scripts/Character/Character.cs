using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;


public class Character : MonoBehaviour, ITakeDamage
{
    [SerializeField] 
    private CharacterType _characterType = CharacterType.Player;

    [SerializeField] 
    private CharacterFaction _faction = CharacterFaction.PlayerTeam;

    [SerializeField] 
    private int _characterId = 0;

    [SerializeField] 
    private CharacterStats _stats;

    [SerializeField] 
    private CharacterAnimationController _characterAnimationController;

    [SerializeField]
    private Rigidbody _rigidbody;

    public CharacterStateMachine StateMachine;
    public CharacterAnimationController AnimationController;
    public CharacterMovementComponent MovementComponent;
    public CharacterAttackComponent AttackComponent;
    public CharacterHealthComponent HealthComponent;
    public CharacterAIController AIController;
    public CharacterInputController InputController;

    // Cache for performance
    private Transform _transform;
    private bool _isInitialized;

    // Identity Properties
    public CharacterType CharacterType => _characterType;
    public CharacterFaction Faction => _faction;
    public int CharacterId => _characterId;
    public string CharacterName { get; private set; }

    // Type Checks
    public bool IsPlayer => _characterType == CharacterType.Player;
    public bool IsEnemy => _characterType == CharacterType.Enemy;
    public bool IsFriendly => _characterType == CharacterType.Friendly;
    public bool IsPlayerTeam => _faction == CharacterFaction.PlayerTeam;
    public bool IsEnemyTeam => _faction == CharacterFaction.EnemyTeam;

    public CharacterStats Stats => _stats;
    public Rigidbody Rigidbody => _rigidbody;
    public Transform Transform => _transform;

    public CharacterStateType CurrentState => StateMachine?.CurrentStateType ?? CharacterStateType.Idle;
    public bool IsInState(CharacterStateType stateType) => CurrentState == stateType;

    public bool IsDead => HealthComponent?.IsDead ?? false;
    public bool IsAlive => !IsDead;
    public bool IsStunned => HealthComponent?.IsStunned ?? false;
    public bool IsMoving => MovementComponent?.IsMoving ?? false;
    public bool IsAttacking => AttackComponent?.IsAttacking ?? false;
    public bool CanMove => MovementComponent?.CanMove ?? true;
    public bool CanAttack => AttackComponent?.CanAttack ?? true;

    public event Action<CharacterStateType, CharacterStateType> OnStateChanged;
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action<Character> OnTargetChanged;

    private List<CharacterComponentBase> _components = new List<CharacterComponentBase>();

    private void Awake()
    {
        _transform = this.transform;
        CharacterName = _stats.CharacterName;
        _components.Add(MovementComponent);
        _components.Add(AttackComponent);
        _components.Add(HealthComponent);

        StateMachine?.Initialize(this);

        if (AIController != null)
        {
            AIController.Initialize(this);
        }

        if (InputController != null)
        {
            InputController.Initialize(this);
        }
    }

    private void Start()
    {
        CompleteInitialization();
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }    

        UpdateSystems(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_isInitialized)
        {
            return;
        }

        FixedUpdateSystems(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        CleanupSystems();
    }

    private void InitializeSystemsWithCharacter()
    {
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].Initialize(this);
        }
    }

    private void CompleteInitialization()
    {
        SubscribeToEvents();
        StateMachine?.ChangeState(CharacterStateType.Idle);

        _isInitialized = true;
    }

    private void UpdateSystems(float deltaTime)
    {
        if (StateMachine != null)
        {
            StateMachine.UpdateLogic(deltaTime);
        }

        if (AIController != null)
        {
            AIController.UpdateLogic(deltaTime);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] != null)
            {
                _components[i].UpdateLogic(deltaTime);
            }
        }
    }

    private void FixedUpdateSystems(float fixedDeltaTime)
    {
        if (StateMachine != null)
        {
            StateMachine.UpdatePhysics(fixedDeltaTime);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] != null)
            {
                _components[i].UpdatePhysics(fixedDeltaTime);
            }
        }
    }
    public void ChangeState(CharacterStateType newState)
    {
        StateMachine?.ChangeState(newState);
    }

    public void ForceChangeState(CharacterStateType newState)
    {
        StateMachine?.ForceChangeState(newState);
    }

    // Movement Control
    public void MoveTo(Vector3 targetPosition)
    {
        MovementComponent?.MoveTo(targetPosition);
    }

    public void LookAt(Vector3 targetPosition)
    {
        MovementComponent?.LookAt(targetPosition);
    }

    public void StopMovement()
    {
        MovementComponent?.Stop();
    }

    // Combat Control
    public void AttackTarget(Character target)
    {
        AttackComponent?.AttackTarget(target);
    }

    public void TakeDamage(float damage, Character attacker = null)
    {
        HealthComponent?.TakeDamage(damage, attacker);
    }

    public void Heal(float amount)
    {
        HealthComponent?.Heal(amount);
    }

    // Character Management
    public void SetCharacterType(CharacterType type)
    {
        _characterType = type;
    }

    public void SetFaction(CharacterFaction faction)
    {
        _faction = faction;
    }

    // Relationship Checks
    public bool IsEnemyOf(Character other)
    {
        if (other == null) return false;
        return _faction != other._faction;
    }

    public bool IsAllyOf(Character other)
    {
        if (other == null || this == other) return false;
        return _faction == other._faction;
    }

    // Utility Methods
    public float GetDistanceTo(Character other)
    {
        if (other == null) return float.MaxValue;
        return Vector3.Distance(_transform.position, other._transform.position);
    }

    public float GetDistanceTo(Vector3 position)
    {
        return Vector3.Distance(_transform.position, position);
    }

    public Vector3 GetDirectionTo(Character other)
    {
        if (other == null) return Vector3.zero;
        return (other._transform.position - _transform.position).normalized;
    }

    public Vector3 GetDirectionTo(Vector3 position)
    {
        return (position - _transform.position).normalized;
    }

    private void SubscribeToEvents()
    {
        if (StateMachine != null)
            StateMachine.OnStateChanged += HandleStateChanged;

        if (HealthComponent != null)
        {
            HealthComponent.OnHealthChanged += HandleHealthChanged;
            HealthComponent.OnDeath += HandleDeath;
            HealthComponent.OnRevive += HandleRevive;
        }
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        OnStateChanged?.Invoke(oldState, newState);
    }

    private void HandleHealthChanged(float newHealth)
    {
        OnHealthChanged?.Invoke(newHealth);
    }

    private void HandleDeath()
    {
        ChangeState(CharacterStateType.Dead);
        OnDeath?.Invoke();
    }

    private void HandleRevive()
    {
        ChangeState(CharacterStateType.Idle);
        OnRevive?.Invoke();
    }

    private void CleanupSystems()
    {
        // Unsubscribe from events
        if (StateMachine != null)
            StateMachine.OnStateChanged -= HandleStateChanged;

        if (HealthComponent != null)
        {
            HealthComponent.OnHealthChanged -= HandleHealthChanged;
            HealthComponent.OnDeath -= HandleDeath;
            HealthComponent.OnRevive -= HandleRevive;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HealthComponent.TakeDamage(damageAmount);
    }
}