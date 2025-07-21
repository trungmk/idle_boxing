using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    private Character _character;
    private bool _isInitialized;

    public void Initialize(Character character)
    {
        _character = character;
        _isInitialized = true;
    }


}