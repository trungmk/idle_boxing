using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ITakeDamage
{
    [SerializeField]
    private CharacterType _characterType = CharacterType.Player;

    [SerializeField]
    private CharacterFaction _faction = CharacterFaction.PlayerTeam;

    [SerializeField]
    private int _characterId = 0;

    [SerializeField]
    private CharacterStats _statsTemplate;

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

    private Transform _transform;
    private bool _isInitialized;
    private CharacterStatsInstance _runtimeStats;
    private List<CharacterComponentBase> _components = new List<CharacterComponentBase>();

    public CharacterType CharacterType => _characterType;
    public CharacterFaction Faction => _faction;
    public int CharacterId => _characterId;
    public string CharacterName { get; private set; }

    public bool IsPlayer => _characterType == CharacterType.Player;
    public bool IsEnemy => _characterType == CharacterType.Enemy;
    public bool IsFriendly => _characterType == CharacterType.Friendly;
    public bool IsPlayerTeam => _faction == CharacterFaction.PlayerTeam;
    public bool IsEnemyTeam => _faction == CharacterFaction.EnemyTeam;

    public CharacterStats Stats => _statsTemplate;
    public CharacterStatsInstance RuntimeStats => _runtimeStats;
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

    private void Awake()
    {
        _transform = transform;
        InitializeRuntimeStats();
        InitializeComponents();
        SubscribeToEvents();
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

    private void InitializeRuntimeStats()
    {
        if (_statsTemplate != null)
        {
            _runtimeStats = new CharacterStatsInstance(_statsTemplate);
            CharacterName = _runtimeStats.CharacterName;
        }
        else
        {
            _runtimeStats = new CharacterStatsInstance();
            CharacterName = _runtimeStats.CharacterName;
        }
    }

    private void InitializeComponents()
    {
        if (MovementComponent != null) _components.Add(MovementComponent);
        if (AttackComponent != null) _components.Add(AttackComponent);
        if (HealthComponent != null) _components.Add(HealthComponent);

        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].Initialize(this);
        }

        if (StateMachine != null) StateMachine.Initialize(this);
        if (AIController != null) AIController.Initialize(this);
        if (InputController != null) InputController.Initialize(this);
    }

    private void CompleteInitialization()
    {
        if (StateMachine != null)
        {
            StateMachine.ChangeState(CharacterStateType.Idle);
        }
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

    public void AttackTarget(Character target)
    {
        AttackComponent?.AttackTarget(target);
    }

    public void TakeDamage(float damage, Character attacker = null)
    {
        HealthComponent?.TakeDamage(damage, attacker);
    }

    public void TakeDamage(int damageAmount)
    {
        HealthComponent?.TakeDamage(damageAmount);
    }

    public void Heal(float amount)
    {
        HealthComponent?.Heal(amount);
    }

    public void SetCharacterType(CharacterType type)
    {
        _characterType = type;
    }

    public void SetFaction(CharacterFaction faction)
    {
        _faction = faction;
    }

    public void AssignStats(CharacterStatsInstance newStats)
    {
        _runtimeStats = newStats;
        CharacterName = _runtimeStats.CharacterName;

        if (_isInitialized)
        {
            ReinitializeWithNewStats();
        }
    }

    public void AssignStatsFromLevel(LevelDataInstance levelData)
    {
        if (_statsTemplate != null && levelData != null)
        {
            _runtimeStats = new CharacterStatsInstance(levelData, _statsTemplate);
            CharacterName = _runtimeStats.CharacterName;

            if (_isInitialized)
            {
                ReinitializeWithNewStats();
            }
        }
    }

    public void UpdateStatsFromTemplate()
    {
        if (_statsTemplate != null)
        {
            _runtimeStats.CopyFromCharacterStats(_statsTemplate);
            CharacterName = _runtimeStats.CharacterName;

            if (_isInitialized)
            {
                ReinitializeWithNewStats();
            }
        }
    }

    private void ReinitializeWithNewStats()
    {
        if (HealthComponent != null)
        {
            HealthComponent.Initialize(this);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] != null)
            {
                _components[i].Initialize(this);
            }
        }
    }

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

    public Character FindNearestEnemy(float searchRadius = 10f)
    {
        Collider[] colliders = Physics.OverlapSphere(_transform.position, searchRadius);
        Character nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Character other = collider.GetComponent<Character>();
            if (other != null && IsEnemyOf(other) && other.IsAlive)
            {
                float distance = GetDistanceTo(other);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = other;
                }
            }
        }

        return nearestEnemy;
    }

    public List<Character> FindEnemiesInRange(float searchRadius = 10f)
    {
        List<Character> enemies = new List<Character>();
        Collider[] colliders = Physics.OverlapSphere(_transform.position, searchRadius);

        foreach (var collider in colliders)
        {
            Character other = collider.GetComponent<Character>();
            if (other != null && IsEnemyOf(other) && other.IsAlive)
            {
                enemies.Add(other);
            }
        }

        return enemies;
    }

    public List<Character> FindAlliesInRange(float searchRadius = 10f)
    {
        List<Character> allies = new List<Character>();
        Collider[] colliders = Physics.OverlapSphere(_transform.position, searchRadius);

        foreach (var collider in colliders)
        {
            Character other = collider.GetComponent<Character>();
            if (other != null && IsAllyOf(other) && other.IsAlive)
            {
                allies.Add(other);
            }
        }

        return allies;
    }

    public void ApplyStun(float duration)
    {
        HealthComponent?.ApplyStun(duration);
    }

    public void Revive(float healthPercentage = 1f)
    {
        if (HealthComponent != null && IsDead)
        {
            HealthComponent.Revive(healthPercentage);
        }
    }

    public void ResetToIdle()
    {
        StopMovement();
        ChangeState(CharacterStateType.Idle);
    }

    public void EnableCharacter()
    {
        gameObject.SetActive(true);
        if (_isInitialized)
        {
            ResetToIdle();
        }
    }

    public void DisableCharacter()
    {
        StopMovement();
        gameObject.SetActive(false);
    }

    private void SubscribeToEvents()
    {
        if (StateMachine != null)
        {
            StateMachine.OnStateChanged += HandleStateChanged;
        }

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
        if (StateMachine != null)
        {
            StateMachine.OnStateChanged -= HandleStateChanged;
        }

        if (HealthComponent != null)
        {
            HealthComponent.OnHealthChanged -= HandleHealthChanged;
            HealthComponent.OnDeath -= HandleDeath;
            HealthComponent.OnRevive -= HandleRevive;
        }
    }
}