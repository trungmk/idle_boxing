using UnityEngine;

public class CharacterStats : ScriptableObject
{
    public float MaxHealth = 100f;

    public float AttackPower = 20f;

    public float AttackCooldown = 1.5f;

    public float MoveSpeed = 3f;

    public bool HasRangeAttack = false;

    public float RangeAttackPower = 15f;
}