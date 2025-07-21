using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    private Character _character;
    private Dictionary<CharacterStateType, CharacterBaseState> _states;
    private CharacterBaseState _currentState;
    private CharacterStateType _currentStateType = CharacterStateType.Idle;
    private bool _isInitialized;

    public CharacterStateType CurrentStateType => _currentStateType;
    public CharacterBaseState CurrentState => _currentState;
    public bool IsInitialized => _isInitialized;

    public event Action<CharacterStateType, CharacterStateType> OnStateChanged;

    public void Initialize(Character character)
    {
        _character = character;
        _states = new Dictionary<CharacterStateType, CharacterBaseState>();

        InitializeStates();
        _isInitialized = true;
    }

    private void InitializeStates()
    {
        _states[CharacterStateType.Idle] = new IdleState(_character);
        _states[CharacterStateType.Chase] = new ChaseState(_character);
        _states[CharacterStateType.Attack] = new AttackState(_character);
        _states[CharacterStateType.Hit] = new HitState(_character);
        _states[CharacterStateType.Dead] = new DeadState(_character);
        _states[CharacterStateType.Victory] = new WinState(_character);
        _states[CharacterStateType.Stunned] = new StunnedState(_character);

        // Set initial state
        ChangeState(CharacterStateType.Idle);
    }

    public void ChangeState(CharacterStateType newStateType)
    {
        if (!_isInitialized || newStateType == _currentStateType) return;

        var oldStateType = _currentStateType;

        _currentState?.Exit();
        _currentStateType = newStateType;
        _currentState = _states[newStateType];
        _currentState?.Enter();

        OnStateChanged?.Invoke(oldStateType, newStateType);
    }

    public void ForceChangeState(CharacterStateType newStateType)
    {
        if (!_isInitialized) return;

        var oldStateType = _currentStateType;

        _currentState?.Exit();
        _currentStateType = newStateType;
        _currentState = _states[newStateType];
        _currentState?.Enter();

        OnStateChanged?.Invoke(oldStateType, newStateType);
    }

    public void UpdateLogic(float deltaTime)
    {
        _currentState?.UpdateLogic(deltaTime);
    }

    public void UpdatePhysics(float fixedDeltaTime)
    {
        _currentState?.UpdatePhysics(fixedDeltaTime);
    }

    public void FixedUpdateLogic(float deltaTime)
    {
        _currentState?.LateUpdateLogic(deltaTime);
    }

    public T GetState<T>() where T : CharacterBaseState
    {
        foreach (var state in _states.Values)
        {
            if (state is T targetState)
                return targetState;
        }
        return null;
    }
}