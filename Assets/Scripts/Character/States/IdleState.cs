using UnityEngine;
using UnityEngine.TextCore.Text;

public class IdleState : CharacterBaseState
{
    public IdleState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayIdle();
        _character.MovementComponent?.Stop();
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (_character.IsPlayer && _character.InputController != null)
        {
            
        }
        else if (!_character.IsPlayer && _character.AIController != null)
        {
            // For AI
        }
    }
}