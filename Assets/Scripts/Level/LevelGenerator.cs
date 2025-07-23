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