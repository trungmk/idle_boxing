using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] 
    private Animator _animator;

    [SerializeField] 
    private float _defaultCrossFadeTime = 0.1f;

    private Character _character;

    private bool _isInitialized;

    private const string IDLE_STATE = "Idle";
    private const string MOVE_STATE = "Move";
    private const string ATTACK_STATE = "Attack";
    private const string HIT_STATE = "Hit";
    private const string DEAD_STATE = "Dead";
    private const string VICTORY_STATE = "Victory";
    private const string STUNNED_STATE = "Stunned";

    private const string MOVE_SPEED_PARAM = "MoveSpeed";
    private const string ATTACK_SPEED_PARAM = "AttackSpeed";
    private const string IS_MOVING_PARAM = "IsMoving";
    private const string IS_ATTACKING_PARAM = "IsAttacking";
    private const string IS_DEAD_PARAM = "IsDead";
    private const string HEALTH_PERCENTAGE_PARAM = "HealthPercentage";

    private static readonly int _idleHash = Animator.StringToHash(IDLE_STATE);
    private static readonly int _moveHash = Animator.StringToHash(MOVE_STATE);
    private static readonly int _attackHash = Animator.StringToHash(ATTACK_STATE);
    private static readonly int _hitHash = Animator.StringToHash(HIT_STATE);
    private static readonly int _deadHash = Animator.StringToHash(DEAD_STATE);
    private static readonly int _victoryHash = Animator.StringToHash(VICTORY_STATE);
    private static readonly int _stunnedHash = Animator.StringToHash(STUNNED_STATE);

    private static readonly int _moveSpeedHash = Animator.StringToHash(MOVE_SPEED_PARAM);
    private static readonly int _attackSpeedHash = Animator.StringToHash(ATTACK_SPEED_PARAM);
    private static readonly int _isMovingHash = Animator.StringToHash(IS_MOVING_PARAM);
    private static readonly int _isAttackingHash = Animator.StringToHash(IS_ATTACKING_PARAM);
    private static readonly int _isDeadHash = Animator.StringToHash(IS_DEAD_PARAM);
    private static readonly int _healthPercentageHash = Animator.StringToHash(HEALTH_PERCENTAGE_PARAM);

    private CharacterStateType _currentAnimationState = CharacterStateType.Idle;
    private float _currentAnimationSpeed = 1f;
    private Dictionary<string, AnimationClip> _animationClips = new Dictionary<string, AnimationClip>();

    public event Action<string> OnAnimationStarted;
    public event Action<string> OnAnimationCompleted;
    public event Action OnAttackHitFrame;
    public event Action OnFootstep;

    public Animator Animator => _animator;
    public bool IsInitialized => _isInitialized;
    public CharacterStateType CurrentAnimationState => _currentAnimationState;
    public float CurrentAnimationSpeed => _currentAnimationSpeed;

    // Animation state checks
    public bool IsPlayingIdle => IsInState(IDLE_STATE);
    public bool IsPlayingMove => IsInState(MOVE_STATE);
    public bool IsPlayingAttack => IsInState(ATTACK_STATE);
    public bool IsPlayingHit => IsInState(HIT_STATE);
    public bool IsPlayingDead => IsInState(DEAD_STATE);
    public bool IsPlayingVictory => IsInState(VICTORY_STATE);
    public bool IsPlayingStunned => IsInState(STUNNED_STATE);

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void Initialize(Character character)
    {
        _character = character;

        if (_animator == null)
        {
            Debug.LogError($"Animator not found on {gameObject.name}!", this);
            return;
        }

        CacheAnimationClips();
        SetupInitialParameters();
        SubscribeToCharacterEvents();

        _isInitialized = true;
    }

    private void CacheAnimationClips()
    {
        if (_animator.runtimeAnimatorController == null) return;

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (!_animationClips.ContainsKey(clip.name))
            {
                _animationClips.Add(clip.name, clip);
            }
        }
    }

    private void SetupInitialParameters()
    {
        if (!_animator.isActiveAndEnabled)
        {
            return;
        } 

        // Set initial parameter values
        SetFloat(MOVE_SPEED_PARAM, 0f);
        SetFloat(ATTACK_SPEED_PARAM, 1f);
        SetBool(IS_MOVING_PARAM, false);
        SetBool(IS_ATTACKING_PARAM, false);
        SetBool(IS_DEAD_PARAM, false);
        SetFloat(HEALTH_PERCENTAGE_PARAM, 1f);
    }

    private void SubscribeToCharacterEvents()
    {
        if (_character == null) return;

        _character.OnStateChanged += HandleStateChanged;
        _character.OnHealthChanged += HandleHealthChanged;
        _character.OnDeath += HandleDeath;
        _character.OnRevive += HandleRevive;
    }

    public void UpdateLogic(float deltaTime)
    {
        if (!_isInitialized || _animator == null) return;

        UpdateAnimationParameters();
        CheckAnimationEvents();
    }

    private void UpdateAnimationParameters()
    {
        if (_character == null) return;

        // Update movement parameters
        bool isMoving = _character.IsMoving;
        SetBool(IS_MOVING_PARAM, isMoving);

        if (isMoving && _character.MovementComponent != null)
        {
            float moveSpeed = _character.MovementComponent.CurrentSpeed / _character.Stats.MoveSpeed;
            SetFloat(MOVE_SPEED_PARAM, moveSpeed);
        }
        else
        {
            SetFloat(MOVE_SPEED_PARAM, 0f);
        }

        // Update combat parameters
        SetBool(IS_ATTACKING_PARAM, _character.IsAttacking);

        // Update health parameters
        if (_character.HealthComponent != null)
        {
            float healthPercentage = _character.HealthComponent.HealthPercentage;
            SetFloat(HEALTH_PERCENTAGE_PARAM, healthPercentage);
        }

        // Update death state
        SetBool(IS_DEAD_PARAM, _character.IsDead);
    }

    private void CheckAnimationEvents()
    {
        if (!_animator.isActiveAndEnabled) return;

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // Check if animation completed
        if (stateInfo.normalizedTime >= 1f && !_animator.IsInTransition(0))
        {
            string currentStateName = GetCurrentStateName();
            OnAnimationCompleted?.Invoke(currentStateName);
        }
    }

    public void PlayStateAnimation(CharacterStateType stateType, float crossFadeTime = -1f)
    {
        if (crossFadeTime < 0f)
            crossFadeTime = _defaultCrossFadeTime;

        string animationName = GetAnimationNameForState(stateType);
        PlayAnimation(animationName, crossFadeTime);
        _currentAnimationState = stateType;
    }

    public void PlayAnimation(string animationName, float crossFadeTime = 0.1f)
    {
        if (!_isInitialized || _animator == null) return;

        int animationHash = Animator.StringToHash(animationName);

        if (HasParameter(animationName))
        {
            _animator.CrossFade(animationHash, crossFadeTime);
            OnAnimationStarted?.Invoke(animationName);
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        if (!_isInitialized || _animator == null)
        {
            return;
        } 
            
        _currentAnimationSpeed = Mathf.Max(0f, speed);
        _animator.speed = _currentAnimationSpeed;
    }

    public void SetMoveSpeed(float speed)
    {
        SetFloat(MOVE_SPEED_PARAM, speed);
    }

    public void SetAttackSpeed(float speed)
    {
        SetFloat(ATTACK_SPEED_PARAM, speed);
    }

    public void PlayIdle()
    {
        PlayStateAnimation(CharacterStateType.Idle);
    }

    public void PlayMove(float speed = 1f)
    {
        SetMoveSpeed(speed);
        PlayStateAnimation(CharacterStateType.Chase);
    }

    public void PlayAttack(float speed = 1f)
    {
        SetAttackSpeed(speed);
        PlayStateAnimation(CharacterStateType.Attack);
    }

    public void PlayHit()
    {
        PlayStateAnimation(CharacterStateType.Hit);
    }

    public void PlayDead()
    {
        PlayStateAnimation(CharacterStateType.Dead);
    }

    public void PlayVictory()
    {
        PlayStateAnimation(CharacterStateType.Victory);
    }

    public void PlayStunned()
    {
        PlayStateAnimation(CharacterStateType.Stunned);
    }

    public void SetTrigger(string parameterName)
    {
        if (!_isInitialized || _animator == null) return;

        if (HasParameter(parameterName))
        {
            _animator.SetTrigger(parameterName);
        }
    }

    public void SetBool(string parameterName, bool value)
    {
        if (!_isInitialized || _animator == null) return;

        if (HasParameter(parameterName))
        {
            _animator.SetBool(parameterName, value);
        }
    }

    public void SetFloat(string parameterName, float value)
    {
        if (!_isInitialized || _animator == null) return;

        if (HasParameter(parameterName))
        {
            _animator.SetFloat(parameterName, value);
        }
    }

    public void SetInteger(string parameterName, int value)
    {
        if (!_isInitialized || _animator == null) return;

        if (HasParameter(parameterName))
        {
            _animator.SetInteger(parameterName, value);
        }
    }

    public bool IsInState(string stateName)
    {
        if (!_isInitialized || _animator == null) return false;

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName);
    }

    public bool IsAnimationComplete(string stateName)
    {
        if (!_isInitialized || _animator == null) return true;

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f;
    }

    public float GetAnimationLength(string animationName)
    {
        if (_animationClips.TryGetValue(animationName, out AnimationClip clip))
        {
            return clip.length;
        }
        return 0f;
    }

    public float GetCurrentAnimationNormalizedTime()
    {
        if (!_isInitialized || _animator == null) return 0f;

        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public string GetCurrentStateName()
    {
        if (!_isInitialized || _animator == null) return string.Empty;

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return _animator.GetLayerName(0) + "." + stateInfo.ToString();
    }

    private bool HasParameter(string parameterName)
    {
        if (_animator.parameters == null) return false;

        foreach (var parameter in _animator.parameters)
        {
            if (parameter.name == parameterName)
                return true;
        }
        return false;
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        PlayStateAnimation(newState);
    }

    private void HandleHealthChanged(float newHealth)
    {
        if (_character?.HealthComponent != null)
        {
            float healthPercentage = _character.HealthComponent.HealthPercentage;
            SetFloat(HEALTH_PERCENTAGE_PARAM, healthPercentage);
        }
    }

    private void HandleDeath()
    {
        PlayDead();
    }

    private void HandleRevive()
    {
        SetBool(IS_DEAD_PARAM, false);
        PlayIdle();
    }

    public void OnAttackHitFrameEvent()
    {
        if (OnAttackHitFrame != null)
        {
            OnAttackHitFrame();
        }
    }

    public void OnFootstepEvent()
    {
        OnFootstep?.Invoke();
    }

    public void OnAnimationCompleteEvent()
    {
        string currentState = GetCurrentStateName();

        if (OnAnimationCompleted != null)
        {
            OnAnimationCompleted(currentState);
        }
    }

    private string GetAnimationNameForState(CharacterStateType stateType)
    {
        return stateType switch
        {
            CharacterStateType.Idle => IDLE_STATE,
            CharacterStateType.Chase => MOVE_STATE,
            CharacterStateType.Attack => ATTACK_STATE,
            CharacterStateType.Hit => HIT_STATE,
            CharacterStateType.Dead => DEAD_STATE,
            CharacterStateType.Victory => VICTORY_STATE,
            CharacterStateType.Stunned => STUNNED_STATE,
            _ => IDLE_STATE
        };
    }

    private void OnDestroy()
    {
        if (_character != null)
        {
            _character.OnStateChanged -= HandleStateChanged;
            _character.OnHealthChanged -= HandleHealthChanged;
            _character.OnDeath -= HandleDeath;
            _character.OnRevive -= HandleRevive;
        }
    }
}