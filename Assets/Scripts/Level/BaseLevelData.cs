using UnityEngine;

[CreateAssetMenu(fileName = "BaseLevelData", menuName = "Game Data/Level/Base Level Data", order = 1)]
public class BaseLevelData : ScriptableObject
{
    [Header("Level Identity")]
    public int LevelId;
    public string LevelName;
    public string LevelDescription;

    [Header("Combat Configuration")]
    public FightMode FightMode = FightMode.OneVsOne;
    public int PlayerCount = 1;
    public int EnemyCount = 1;

    [Header("Difficulty Settings")]
    [Range(1, 10)]
    public int DifficultyLevel = 1;
    public float EnemyHealthMultiplier = 1f;
    public float EnemyDamageMultiplier = 1f;
    public float EnemySpeedMultiplier = 1f;
    public float EnemyAIAggressiveness = 0.5f;

    [Header("Arena Settings")]
    public string ArenaSceneName = "BoxingRing";
    public Vector3 ArenaSize = new Vector3(10f, 1f, 10f);

    [Header("Spawn Configuration")]
    public Vector3[] PlayerSpawnPoints;
    public Vector3[] EnemySpawnPoints;

    [Header("Rewards")]
    public int ExperienceReward = 100;
    public int CoinReward = 50;

    // Calculate final stats based on difficulty
    public float GetEnemyFinalHealth(float baseHealth) => baseHealth * EnemyHealthMultiplier;
    public float GetEnemyFinalDamage(float baseDamage) => baseDamage * EnemyDamageMultiplier;
    public float GetEnemyFinalSpeed(float baseSpeed) => baseSpeed * EnemySpeedMultiplier;
}