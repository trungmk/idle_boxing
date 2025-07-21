using UnityEngine;
using UnityEngine.TextCore.Text;

public class HitState : CharacterBaseState
{
    private float _hitTimer;
    private const float HIT_DURATION = 0.8f;

    public HitState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayHit();
        _character.MovementComponent?.Stop();
        _hitTimer = 0f;
    }

    public override void UpdateLogic(float deltaTime)
    {
        _hitTimer += deltaTime;

        if (_hitTimer >= HIT_DURATION)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }
}