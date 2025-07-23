using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpawnCharacterManager : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTrans;

    [SerializeField]
    private Transform _friendlyTrans;

    [SerializeField]
    private Transform _enemy1Trans;

    [SerializeField]
    private Transform _enemy2Trans;

    public async UniTaskVoid SpawnCharacters(FightMode fightMode)
    {
        Character player = await ObjectPooling.Instance.Get<Character>("Player");
        player.transform.SetPositionAndRotation(_playerTrans.position, Quaternion.identity);

        Character enemy1 = await ObjectPooling.Instance.Get<Character>("Enemy_1");
        enemy1.transform.SetPositionAndRotation(_enemy1Trans.position, Quaternion.Euler(0, 180f, 0));

        if (fightMode == FightMode.OneVsMany)
        {
            Character enemy2 = await ObjectPooling.Instance.Get<Character>("Enemy_1");
            enemy2.transform.SetPositionAndRotation(_enemy2Trans.position, Quaternion.Euler(0, 180f, 0));
        }

        if (fightMode == FightMode.ManyVsMany)
        {
            Character friendly = await ObjectPooling.Instance.Get<Character>("Player");
            friendly.transform.SetPositionAndRotation(_friendlyTrans.position, Quaternion.identity);

            Character enemy2 = await ObjectPooling.Instance.Get<Character>("Enemy_1");
            enemy2.transform.SetPositionAndRotation(_enemy2Trans.position, Quaternion.Euler(0, 180f, 0));
        }
    }
}