using UnityEngine;

public class CharacterStatsInstance
{
    public string CharacterName;
    public float MaxHealth = 100f;
    public float AttackPower = 20f;
    public float AttackCooldown = 1.5f;
    public float MoveSpeed = 3f;
    public bool HasRangeAttack = false;
    public float RangeAttackPower = 15f;
    public float CriticalChance = 0.05f;
    public float CriticalMultiplier = 1.5f;

    public CharacterStatsInstance()
    {  
    }

    public CharacterStatsInstance(CharacterStats baseStats)
    {
        if (baseStats != null)
        {
            CopyFromCharacterStats(baseStats);
        }
    }

    public CharacterStatsInstance(LevelDataInstance levelData, CharacterStats baseTemplate)
    {
        if (baseTemplate != null && levelData != null)
        {
            CopyFromCharacterStats(baseTemplate);
            ApplyLevelScaling(levelData);
        }
    }

    public void CopyFromCharacterStats(CharacterStats stats)
    {
        CharacterName = stats.CharacterName;
        MaxHealth = stats.MaxHealth;
        AttackPower = stats.AttackPower;
        AttackCooldown = stats.AttackCooldown;
        MoveSpeed = stats.MoveSpeed;
        HasRangeAttack = stats.HasRangeAttack;
        RangeAttackPower = stats.RangeAttackPower;
        CriticalChance = stats.CriticalChance;
        CriticalMultiplier = stats.CriticalMultiplier;
    }

    public void ApplyLevelScaling(LevelDataInstance levelData)
    {
        MaxHealth = levelData.EnemyHealth;
        AttackPower = levelData.EnemyDamage;
        CriticalChance = levelData.EnemyCrit;
        MoveSpeed = levelData.EnemySpeed;
        CharacterName = $"Enemy Level {levelData.LevelId}";
    }

    public CharacterStatsInstance CreateScaledVersion(float healthMultiplier, float damageMultiplier, float critMultiplier = 1f)
    {
        CharacterStatsInstance scaledStats = new CharacterStatsInstance();

        scaledStats.CharacterName = this.CharacterName + " (Scaled)";
        scaledStats.MaxHealth = this.MaxHealth * healthMultiplier;
        scaledStats.AttackPower = this.AttackPower * damageMultiplier;
        scaledStats.RangeAttackPower = this.RangeAttackPower * damageMultiplier;
        scaledStats.CriticalChance = Mathf.Clamp01(this.CriticalChance * critMultiplier);
        scaledStats.CriticalMultiplier = this.CriticalMultiplier;
        scaledStats.HasRangeAttack = this.HasRangeAttack;
        scaledStats.AttackCooldown = this.AttackCooldown;
        scaledStats.MoveSpeed = this.MoveSpeed;

        return scaledStats;
    }

    public float GetMeleeDamage(bool isCritical = false)
    {
        return isCritical ? AttackPower * CriticalMultiplier : AttackPower;
    }

    public float GetRangeDamage(bool isCritical = false)
    {
        if (!HasRangeAttack) return 0f;
        return isCritical ? RangeAttackPower * CriticalMultiplier : RangeAttackPower;
    }

    public bool RollCriticalHit()
    {
        return UnityEngine.Random.value <= CriticalChance;
    }

    public float GetDamagePerSecond()
    {
        float attacksPerSecond = 1f / AttackCooldown;
        float averageDamage = AttackPower * (1f + CriticalChance * (CriticalMultiplier - 1f));
        return averageDamage * attacksPerSecond;
    }

    public float GetEffectiveHealth()
    {
        return MaxHealth;
    }

    public float GetCombatRating()
    {
        float dps = GetDamagePerSecond();
        float sustainability = GetEffectiveHealth() / 100f;
        return dps * sustainability;
    }

    public override string ToString()
    {
        return $"{CharacterName}: HP={MaxHealth:F0}, ATK={AttackPower:F0}, CRIT={CriticalChance * 100:F1}%, SPD={MoveSpeed:F1}";
    }
}