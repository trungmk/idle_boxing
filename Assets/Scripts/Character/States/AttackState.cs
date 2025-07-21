using UnityEngine;
using UnityEngine.TextCore.Text;

public class AttackState : CharacterBaseState
{
    private float _attackTimer;
    private const float ATTACK_DURATION = 1f;

    public AttackState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayAttack();
        _character.MovementController?.Stop();
        _attackTimer = 0f;
    }

    public override void UpdateLogic(float deltaTime)
    {
        _attackTimer += deltaTime;

        if (_attackTimer >= ATTACK_DURATION)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }
}