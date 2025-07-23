using UnityEngine;

[CreateAssetMenu(fileName = "AvatarSprites", menuName = "Game Data/Sprite/AvatarSpritesSO", order = 1)]
public class AvatarSpritesSO : ScriptableObject
{
    public Sprite Player;
    
    public Sprite Player2;

    public Sprite Enemy1;

    public Sprite Enemy2;

    public Sprite GetSpriteByName(string name)
    {
        switch (name)
        {
            case "Player":
                return Player;

            case "Player2":
                return Player2;

            case "Enemy_1":
                return Enemy1;

            case "Enemy_2":
                return Enemy2;
            default:
                Debug.LogWarning($"Avatar sprite with name '{name}' not found.");
                return null;
        }
    }
}
