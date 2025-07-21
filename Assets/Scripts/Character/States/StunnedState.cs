using UnityEngine;
using UnityEngine.TextCore.Text;

public class StunnedState : CharacterBaseState
{
    public StunnedState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayStunned();
        _character.MovementComponent?.Stop();
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (!_character.IsStunned)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }
}