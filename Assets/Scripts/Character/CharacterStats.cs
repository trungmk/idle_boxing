using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Game Data/Character/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    public string CharacterName = "Default Character";

    public float MaxHealth = 100f;

    public float AttackPower = 20f;

    public float AttackCooldown = 1.5f;

    public float MoveSpeed = 3f;

    public bool HasRangeAttack = false;

    public float RangeAttackPower = 15f;

    public float CriticalChance = 0.05f;
    
    public float CriticalMultiplier = 1.5f;

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
        return Random.value <= CriticalChance;
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

    public CharacterStats CreateScaledVersion(float healthMultiplier, float damageMultiplier, float critMultiplier = 1f)
    {
        CharacterStats scaledStats = CreateInstance<CharacterStats>();
        scaledStats.CharacterName = this.CharacterName;
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
}