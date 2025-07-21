using UnityEngine;
using UnityEngine.TextCore.Text;

public class DeadState : CharacterBaseState
{
    public DeadState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayDead();
        _character.MovementController?.Stop();

        if (_character.Collider != null)
            _character.Collider.enabled = false;
    }

    public override void Exit()
    {
        if (_character.Collider != null)
            _character.Collider.enabled = true;
    }
}