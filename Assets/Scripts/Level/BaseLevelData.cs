using UnityEngine;

[CreateAssetMenu(fileName = "BaseLevelData", menuName = "Game Data/Level/Base Level Data", order = 1)]
public class BaseLevelData : ScriptableObject
{
    public int LevelId;
    public float EnemyHealth = 1f;
    public float EnemyDamage = 1f;
    public float EnemyCrit = 0.05f;
    public float EnemySpeed = 1f;
}