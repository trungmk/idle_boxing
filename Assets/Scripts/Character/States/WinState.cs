using UnityEngine;
using UnityEngine.TextCore.Text;

public class WinState : CharacterBaseState
{
    private float _victoryTimer;
    private const float VICTORY_DURATION = 3f;

    public WinState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.AnimationController?.PlayVictory();
        _character.MovementController?.Stop();
        _victoryTimer = 0f;
    }

    public override void UpdateLogic(float deltaTime)
    {
        _victoryTimer += deltaTime;

        if (_victoryTimer >= VICTORY_DURATION)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }
}