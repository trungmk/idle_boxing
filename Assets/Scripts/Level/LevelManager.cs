using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoSingleton<LevelManager>
{
    public LevelGenerator LevelGenerator;
    public int MaxCachedLevels = 20;
    public FightMode CurrentFightMode { get; set; } = FightMode.OneVsOne;

    private readonly Dictionary<int, LevelDataInstance> _levelCache = new Dictionary<int, LevelDataInstance>();
    private readonly Dictionary<int, CharacterStatsInstance> _enemyStatsCache = new Dictionary<int, CharacterStatsInstance>();

    public LevelDataInstance GetLevel(int levelNumber)
    {
        if (_levelCache.TryGetValue(levelNumber, out LevelDataInstance cachedLevel))
        {
            return cachedLevel;
        }

        LevelDataInstance newLevel = LevelGenerator.GenerateLevel(levelNumber);
        CacheLevel(levelNumber, newLevel);

        return newLevel;
    }

    public CharacterStatsInstance GetEnemyStats(int levelNumber)
    {
        if (_enemyStatsCache.TryGetValue(levelNumber, out CharacterStatsInstance cachedStats))
        {
            return cachedStats;
        }

        LevelDataInstance levelData = GetLevel(levelNumber);
        CharacterStatsInstance newStats = new CharacterStatsInstance(levelData, LevelGenerator.BaseEnemyStats);
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

    private void CacheEnemyStats(int levelNumber, CharacterStatsInstance stats)
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