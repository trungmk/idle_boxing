using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChaseState : CharacterBaseState
{
    public ChaseState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayMove();
    }

    public override void UpdateLogic(float deltaTime)
    {
        
    }

    public override void Exit()
    {
        _character.MovementComponent?.Stop();
    }
}