using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public LevelGenerator LevelGenerator;
    public int MaxCachedLevels = 20;

    private readonly Dictionary<int, LevelDataInstance> _levelCache = new Dictionary<int, LevelDataInstance>();
    private readonly Dictionary<int, CharacterStats> _enemyStatsCache = new Dictionary<int, CharacterStats>();

    public LevelDataInstance GetLevel(int levelNumber)
    {
        if (_levelCache.TryGetValue(levelNumber, out LevelDataInstance cachedLevel))
        {
            return cachedLevel;
        }

        // Generate and cache
        LevelDataInstance newLevel = LevelGenerator.GenerateLevel(levelNumber);
        CacheLevel(levelNumber, newLevel);

        return newLevel;
    }

    public CharacterStats GetEnemyStats(int levelNumber)
    {
        if (_enemyStatsCache.TryGetValue(levelNumber, out CharacterStats cachedStats))
        {
            return cachedStats;
        }

        // Generate and cache
        CharacterStats newStats = LevelGenerator.CreateEnemyStatsForLevel(levelNumber);
        CacheEnemyStats(levelNumber, newStats);

        return newStats;
    }

    private void CacheLevel(int levelNumber, LevelDataInstance levelData)
    {
        if (_levelCache.Count >= MaxCachedLevels)
        {
            ClearOldestCache();
        }

        _levelCache[levelNumber] = levelData;
    }

    private void CacheEnemyStats(int levelNumber, CharacterStats stats)
    {
        if (_enemyStatsCache.Count >= MaxCachedLevels)
        {
            ClearOldestStatsCache();
        }

        _enemyStatsCache[levelNumber] = stats;
    }

    private void ClearOldestCache()
    {
        var enumerator = _levelCache.GetEnumerator();
        if (enumerator.MoveNext())
        {
            _levelCache.Remove(enumerator.Current.Key);
        }
    }

    private void ClearOldestStatsCache()
    {
        var enumerator = _enemyStatsCache.GetEnumerator();
        if (enumerator.MoveNext())
        {
            _enemyStatsCache.Remove(enumerator.Current.Key);
        }
    }

    public void ClearCache()
    {
        _levelCache.Clear();
        _enemyStatsCache.Clear();
    }
}