using UnityEngine;

public abstract class CharacterComponentBase : MonoBehaviour
{
    protected Character Character { get; private set; }

    public virtual void Initialize(Character character)
    {
        Character = character;
    }

    public virtual void UpdateLogic(float deltaTime)
    {
        
    }

    public virtual void UpdatePhysics(float fixedDeltaTime)
    {
        
    }

    public virtual void LateUpdateLogic(float deltaTime)
    {

    }
}