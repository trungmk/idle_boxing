using Core;
using UnityEngine;

public class GameSceneController : SceneController
{
    [SerializeField]
    private GameManager _gameManager;

    public override void OnLoaded()
    {
        _gameManager.InitGame();
    }
}
