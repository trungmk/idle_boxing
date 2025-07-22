using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelGenerator", menuName = "Game Data/Level/Level Generator", order = 2)]
public class LevelGenerator : ScriptableObject
{
    [Header("Generation Settings")]
    public BaseLevelData BaseLevelTemplate;
    public int TotalLevels = 10;

    [Header("Difficulty Progression")]
    public AnimationCurve HealthMultiplierCurve = AnimationCurve.Linear(0, 1f, 1f, 3f);
    public AnimationCurve DamageMultiplierCurve = AnimationCurve.Linear(0, 1f, 1f, 2.5f);
    public AnimationCurve SpeedMultiplierCurve = AnimationCurve.Linear(0, 1f, 1f, 1.5f);
    public AnimationCurve AggressivenessCurve = AnimationCurve.Linear(0, 0.3f, 1f, 1f);

    [Header("Combat Mode Progression")]
    public CombatModeProgression[] CombatModeProgressions;

    public List<BaseLevelData> GenerateAllLevels()
    {
        List<BaseLevelData> generatedLevels = new List<BaseLevelData>();

        for (int i = 0; i < TotalLevels; i++)
        {
            BaseLevelData levelData = GenerateLevel(i + 1);
            generatedLevels.Add(levelData);
        }

        return generatedLevels;
    }

    private BaseLevelData GenerateLevel(int levelNumber)
    {
        BaseLevelData levelData = CreateInstance<BaseLevelData>();

        // Basic Info
        levelData.LevelId = levelNumber;
        levelData.LevelName = $"Level {levelNumber}";
        levelData.LevelDescription = GenerateLevelDescription(levelNumber);
        levelData.DifficultyLevel = Mathf.Clamp(levelNumber, 1, 10);

        // Difficulty progression (0-1 normalized)
        float progression = (levelNumber - 1f) / (TotalLevels - 1f);

        // Apply difficulty curves
        levelData.EnemyHealthMultiplier = HealthMultiplierCurve.Evaluate(progression);
        levelData.EnemyDamageMultiplier = DamageMultiplierCurve.Evaluate(progression);
        levelData.EnemySpeedMultiplier = SpeedMultiplierCurve.Evaluate(progression);
        levelData.EnemyAIAggressiveness = AggressivenessCurve.Evaluate(progression);

        // Combat mode progression
        CombatModeProgression modeProgression = GetCombatModeForLevel(levelNumber);
        levelData.PlayerCount = modeProgression.PlayerCount;
        levelData.EnemyCount = modeProgression.EnemyCount;

        // Generate spawn points
        GenerateSpawnPoints(levelData);

        // Rewards scaling
        levelData.ExperienceReward = Mathf.RoundToInt(100 * (1 + progression * 2));
        levelData.CoinReward = Mathf.RoundToInt(50 * (1 + progression * 3));

        return levelData;
    }

    private string GenerateLevelDescription(int levelNumber)
    {
        string[] descriptions = {
            "Basic training in the ring",
            "Face your first real opponent",
            "Multiple enemies appear",
            "Enhanced enemy skills",
            "Aggressive opponents",
            "Survival challenge",
            "Elite fighters",
            "Championship round",
            "Ultimate challenge",
            "Legendary showdown"
        };

        return descriptions[Mathf.Clamp(levelNumber - 1, 0, descriptions.Length - 1)];
    }

    private CombatModeProgression GetCombatModeForLevel(int levelNumber)
    {
        foreach (var progression in CombatModeProgressions)
        {
            if (levelNumber >= progression.StartLevel && levelNumber <= progression.EndLevel)
            {
                return progression;
            }
        }

        // Default fallback
        return new CombatModeProgression
        {
            PlayerCount = 1,
            EnemyCount = 1
        };
    }

    private void GenerateSpawnPoints(BaseLevelData levelData)
    {
        Vector3 arenaSize = BaseLevelTemplate.ArenaSize;

        // Generate player spawn points
        levelData.PlayerSpawnPoints = new Vector3[levelData.PlayerCount];
        for (int i = 0; i < levelData.PlayerCount; i++)
        {
            float angle = (360f / levelData.PlayerCount) * i;
            Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * (arenaSize.x * 0.3f),
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * (arenaSize.z * 0.3f)
            );
            levelData.PlayerSpawnPoints[i] = position;
        }

        // Generate enemy spawn points
        levelData.EnemySpawnPoints = new Vector3[levelData.EnemyCount];
        for (int i = 0; i < levelData.EnemyCount; i++)
        {
            float angle = (360f / levelData.EnemyCount) * i + 180f; // Opposite side
            Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * (arenaSize.x * 0.4f),
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * (arenaSize.z * 0.4f)
            );
            levelData.EnemySpawnPoints[i] = position;
        }
    }
}

[System.Serializable]
public class CombatModeProgression
{
    public int StartLevel = 1;
    public int EndLevel = 10;
    public int PlayerCount = 1;
    public int EnemyCount = 1;
}