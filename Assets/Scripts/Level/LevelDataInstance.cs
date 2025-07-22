using UnityEngine;

[System.Serializable]
public class LevelDataInstance
{
    public int LevelId;
    public float EnemyHealth;
    public float EnemyDamage;
    public float EnemyCrit;
    public float EnemySpeed;

    public LevelDataInstance(int levelId, float health, float damage, float crit, float speed)
    {
        LevelId = levelId;
        EnemyHealth = health;
        EnemyDamage = damage;
        EnemyCrit = crit;
        EnemySpeed = speed;
    }

    public LevelDataInstance()
    {
        LevelId = 1;
        EnemyHealth = 100f;
        EnemyDamage = 20f;
        EnemyCrit = 0.05f;
        EnemySpeed = 3f;
    }

    public override string ToString()
    {
        return $"Level {LevelId}: HP={EnemyHealth:F0}, DMG={EnemyDamage:F0}, CRIT={EnemyCrit * 100:F1}%, SPD={EnemySpeed:F1}";
    }
}