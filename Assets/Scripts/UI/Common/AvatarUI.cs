using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    [SerializeField]
    private Image _avatarImage;

    public void SetAvatar(Sprite avatarSprite)
    {
        if (_avatarImage != null && avatarSprite != null)
        {
            _avatarImage.sprite = avatarSprite;
            _avatarImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Avatar image or sprite is null.");
        }
    }
}
