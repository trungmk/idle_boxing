using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SpawnCharacterManager _spawnCharacterManager;

    public void InitGame()
    {
        _spawnCharacterManager.SpawnCharacters(LevelManager.Instance.CurrentFightMode).Forget();
    }
}
