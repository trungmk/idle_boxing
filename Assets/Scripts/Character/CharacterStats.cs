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
}