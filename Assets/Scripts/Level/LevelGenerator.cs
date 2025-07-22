using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelGenerator", menuName = "Game Data/Level/Level Generator", order = 2)]
public class LevelGenerator : ScriptableObject
{
    public CharacterStats BaseEnemyStats;
    public float HealthScalingPerLevel = 0.4f;
    public float DamageScalingPerLevel = 0.3f;
    public float CritScalingPerLevel = 0.01f; 
    public float SpeedScalingPerLevel = 0.2f;

    // Generate runtime level data (no ScriptableObject creation)
    public LevelDataInstance GenerateLevel(int levelNumber)
    {
        if (BaseEnemyStats == null)
        {
            Debug.LogError("BaseEnemyStats not assigned in LevelGenerator!");
            return new LevelDataInstance(); 
        }

        float levelMultiplier = levelNumber - 1;
        float enemyHealth = BaseEnemyStats.MaxHealth * (1f + HealthScalingPerLevel * levelMultiplier);
        float enemyDamage = BaseEnemyStats.AttackPower * (1f + DamageScalingPerLevel * levelMultiplier);
        float enemyCrit = Mathf.Clamp01(BaseEnemyStats.CriticalChance + (CritScalingPerLevel * levelMultiplier));
        float enemySpeed = BaseEnemyStats.MoveSpeed * (1f + SpeedScalingPerLevel * levelMultiplier);

        return new LevelDataInstance(levelNumber, enemyHealth, enemyDamage, enemyCrit, enemySpeed);
    }

    public CharacterStats CreateEnemyStatsForLevel(int levelNumber)
    {
        if (BaseEnemyStats == null)
        {
            Debug.LogError("BaseEnemyStats not assigned!");
            return null;
        }

        LevelDataInstance levelData = GenerateLevel(levelNumber);
        float healthMultiplier = levelData.EnemyHealth / BaseEnemyStats.MaxHealth;
        float damageMultiplier = levelData.EnemyDamage / BaseEnemyStats.AttackPower;
        float critMultiplier = levelData.EnemyCrit / BaseEnemyStats.CriticalChance;

        CharacterStats enemyStats = BaseEnemyStats.CreateScaledVersion(healthMultiplier, damageMultiplier, critMultiplier);
        enemyStats.MoveSpeed = levelData.EnemySpeed;
        enemyStats.CharacterName = $"Enemy Level {levelNumber}";

        return enemyStats;
    }

    public List<LevelDataInstance> GenerateLevels(int startLevel, int endLevel)
    {
        List<LevelDataInstance> levels = new List<LevelDataInstance>();

        for (int i = startLevel; i <= endLevel; i++)
        {
            levels.Add(GenerateLevel(i));
        }

        return levels;
    }
}