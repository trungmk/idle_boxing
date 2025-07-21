using UnityEngine;
using UnityEngine.TextCore.Text;

public class DeadState : CharacterBaseState
{
    public DeadState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayDead();
        _character.MovementComponent?.Stop();

        
    }

    public override void Exit()
    {

    }
}