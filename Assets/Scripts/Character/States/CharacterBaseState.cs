using UnityEngine;

public abstract class CharacterBaseState
{
    protected Character _character;

    public CharacterBaseState(Character character)
    {
        _character = character;
    }

    public virtual void Enter() { }

    public virtual void UpdateLogic(float deltaTime) { }

    public virtual void UpdatePhysics(float fixedDeltaTime) { }

    public virtual void LateUpdateLogic(float deltaTime) { }

    public virtual void Exit() { }
}