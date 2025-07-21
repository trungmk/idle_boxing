public enum CharacterStateType : byte
{
    Idle = 0,
    Chase,
    Attack,
    Hit,
    Dead,
    Victory,
    Stunned
}

public enum CharacterType : byte
{
    Player = 0,
    Enemy,
    Friendly
}

public enum CharacterFaction : byte
{
    PlayerTeam = 0,
    EnemyTeam = 1,
    NeutralTeam = 2
}