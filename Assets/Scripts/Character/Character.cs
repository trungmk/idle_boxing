using System;
using UnityEngine;

public class Character : MonoBehaviour, ITakeDamage
{
    [SerializeField] private CharacterType _characterType = CharacterType.Player;
    [SerializeField] private CharacterFaction _faction = CharacterFaction.PlayerTeam;
    [SerializeField] private int _characterId = 0;
    [SerializeField] private string _characterName = "Character";

    [SerializeField] private CharacterStats _stats;
    [SerializeField] private CharacterAnimationController _characterAnimationController;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    // Core Systems
    private CharacterStateMachine _stateMachine;
    private CharacterMovementComponent _movementComponent;
    private CharacterCombatController _combatController;
    private CharacterHealthController _healthComponent;
    private CharacterAIController _aiController;
    private CharacterInputController _inputController;

    // Cache for performance
    private Transform _transform;
    private bool _isInitialized;

    #region Properties

    // Identity Properties
    public CharacterType CharacterType => _characterType;
    public CharacterFaction Faction => _faction;
    public int CharacterId => _characterId;
    public string CharacterName => _characterName;

    // Type Checks
    public bool IsPlayer => _characterType == CharacterType.Player;
    public bool IsEnemy => _characterType == CharacterType.Enemy;
    public bool IsFriendly => _characterType == CharacterType.Friendly;
    public bool IsPlayerTeam => _faction == CharacterFaction.PlayerTeam;
    public bool IsEnemyTeam => _faction == CharacterFaction.EnemyTeam;

    // Core Component Access
    public CharacterStats Stats => _stats;
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public Collider Collider => _collider;
    public Transform Transform => _transform;

    // System Access
    public CharacterStateMachine StateMachine => _stateMachine;
    public CharacterAnimationController AnimationController => _animationController;
    public CharacterMovementController MovementController => _movementComponent;
    public CharacterCombatController CombatController => _combatController;
    public CharacterHealthComponent HealthComponent => _healthComponent;
    public CharacterAIController AIController => _aiController;
    public CharacterInputController InputController => _inputController;

    // State Access
    public CharacterStateType CurrentState => _stateMachine?.CurrentStateType ?? CharacterStateType.Idle;
    public bool IsInState(CharacterStateType stateType) => CurrentState == stateType;

    // Status Properties
    public bool IsDead => _healthComponent?.IsDead ?? false;
    public bool IsAlive => !IsDead;
    public bool IsStunned => _healthComponent?.IsStunned ?? false;
    public bool IsMoving => _movementComponent?.IsMoving ?? false;
    public bool IsAttacking => _combatController?.IsAttacking ?? false;
    public bool CanMove => _movementComponent?.CanMove ?? true;
    public bool CanAttack => _combatController?.CanAttack ?? true;

    // Events
    public event Action<CharacterStateType, CharacterStateType> OnStateChanged;
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action<Character> OnTargetChanged;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        _transform = transform;
        CacheComponents();
        InitializeSystems();
    }

    private void Start()
    {
        CompleteInitialization();
    }

    private void Update()
    {
        if (!_isInitialized) return;

        UpdateSystems(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_isInitialized) return;

        FixedUpdateSystems(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        CleanupSystems();
    }

    #endregion

    #region Initialization

    private void CacheComponents()
    {
        if (_stats == null) _stats = GetComponent<CharacterStats>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_collider == null) _collider = GetComponent<Collider>();
    }

    private void InitializeSystems()
    {
        // Initialize core systems
        _stateMachine = GetOrAddComponent<CharacterStateMachine>();
        _animationController = GetOrAddComponent<CharacterAnimationController>();
        _movementComponent = GetOrAddComponent<CharacterMovementController>();
        _combatController = GetOrAddComponent<CharacterCombatController>();
        _healthComponent = GetOrAddComponent<CharacterHealthController>();

        // Initialize AI or Input based on character type
        if (IsPlayer)
        {
            _inputController = GetOrAddComponent<CharacterInputController>();
        }
        else
        {
            _aiController = GetOrAddComponent<CharacterAIController>();
        }

        // Initialize all systems with this character reference
        InitializeSystemsWithCharacter();
    }

    private void InitializeSystemsWithCharacter()
    {
        _stateMachine?.Initialize(this);
        _animationController?.Initialize(this);
        _movementComponent?.Initialize(this);
        _combatController?.Initialize(this);
        _healthComponent?.Initialize(this);
        _aiController?.Initialize(this);
        _inputController?.Initialize(this);
    }

    private void CompleteInitialization()
    {
        // Subscribe to events
        SubscribeToEvents();

        // Set initial state
        _stateMachine?.ChangeState(CharacterStateType.Idle);

        _isInitialized = true;
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }

    #endregion

    #region System Updates

    private void UpdateSystems(float deltaTime)
    {
        _stateMachine?.UpdateLogic(deltaTime);
        _animationController?.UpdateLogic(deltaTime);
        _movementComponent?.UpdateLogic(deltaTime);
        _combatController?.UpdateLogic(deltaTime);
        _healthComponent?.UpdateLogic(deltaTime);
        _aiController?.UpdateLogic(deltaTime);
        _inputController?.UpdateLogic(deltaTime);
    }

    private void FixedUpdateSystems(float fixedDeltaTime)
    {
        _stateMachine?.UpdatePhysics(fixedDeltaTime);
        _movementComponent?.UpdatePhysics(fixedDeltaTime);
        _combatController?.UpdatePhysics(fixedDeltaTime);
    }

    #endregion

    #region Public API

    // State Management
    public void ChangeState(CharacterStateType newState)
    {
        _stateMachine?.ChangeState(newState);
    }

    public void ForceChangeState(CharacterStateType newState)
    {
        _stateMachine?.ForceChangeState(newState);
    }

    // Animation Control
    public void PlayAnimation(string animationName, float crossFadeTime = 0.1f)
    {
        _animationController?.PlayAnimation(animationName, crossFadeTime);
    }

    public void SetAnimationSpeed(float speed)
    {
        _animationController?.SetAnimationSpeed(speed);
    }

    // Movement Control
    public void MoveTo(Vector3 targetPosition)
    {
        _movementComponent?.MoveTo(targetPosition);
    }

    public void LookAt(Vector3 targetPosition)
    {
        _movementComponent?.LookAt(targetPosition);
    }

    public void StopMovement()
    {
        _movementComponent?.Stop();
    }

    // Combat Control
    public void AttackTarget(Character target)
    {
        _combatController?.AttackTarget(target);
    }

    public void TakeDamage(float damage, Character attacker = null)
    {
        _healthComponent?.TakeDamage(damage, attacker);
    }

    public void Heal(float amount)
    {
        _healthComponent?.Heal(amount);
    }

    // Character Management
    public void SetCharacterType(CharacterType type)
    {
        _characterType = type;
        ReconfigureForType();
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

    #endregion

    #region Event Handling

    private void SubscribeToEvents()
    {
        if (_stateMachine != null)
            _stateMachine.OnStateChanged += HandleStateChanged;

        if (_healthComponent != null)
        {
            _healthComponent.OnHealthChanged += HandleHealthChanged;
            _healthComponent.OnDeath += HandleDeath;
            _healthComponent.OnRevive += HandleRevive;
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

    #endregion

    #region Private Methods

    private void ReconfigureForType()
    {
        // Remove old controller
        if (_inputController != null && !IsPlayer)
        {
            DestroyImmediate(_inputController);
            _inputController = null;
        }

        if (_aiController != null && IsPlayer)
        {
            DestroyImmediate(_aiController);
            _aiController = null;
        }

        // Add appropriate controller
        if (IsPlayer && _inputController == null)
        {
            _inputController = GetOrAddComponent<CharacterInputController>();
            _inputController.Initialize(this);
        }
        else if (!IsPlayer && _aiController == null)
        {
            _aiController = GetOrAddComponent<CharacterAIController>();
            _aiController.Initialize(this);
        }
    }

    private void CleanupSystems()
    {
        // Unsubscribe from events
        if (_stateMachine != null)
            _stateMachine.OnStateChanged -= HandleStateChanged;

        if (_healthComponent != null)
        {
            _healthComponent.OnHealthChanged -= HandleHealthChanged;
            _healthComponent.OnDeath -= HandleDeath;
            _healthComponent.OnRevive -= HandleRevive;
        }
    }

    #endregion

    #region Debug

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugLog(string message)
    {
        Debug.Log($"[{_characterName}] {message}", this);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw character info
        if (_stats != null)
        {
            Gizmos.color = IsPlayer ? Color.green : (IsEnemy ? Color.red : Color.blue);
            Gizmos.DrawWireSphere(transform.position, _stats.DetectionRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _stats.AttackRange);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        _healthComponent.TakeDamage(damageAmount);
    }

    #endregion
}