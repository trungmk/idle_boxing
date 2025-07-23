using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField]
    private AvatarSpritesSO _avatarSprites;

    [SerializeField]
    private HealthBarUI _healthBar;

    [SerializeField]
    private AvatarUI _avatarUI;

    private CharacterStatsInstance _characterStats;

    public void InitCharacterInfo(CharacterStatsInstance characterStatsInstance)
    {
        _characterStats = characterStatsInstance;
        _avatarUI.SetAvatar(_avatarSprites.GetSpriteByName(_characterStats.CharacterName));

        _healthBar.Initialize(characterStatsInstance.MaxHealth);
    }

    public void UpdateHealth(float currentHealth)
    {
        _healthBar.UpdateHealth(currentHealth, _characterStats.MaxHealth);
    }
}
