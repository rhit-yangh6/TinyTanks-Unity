namespace _Scripts.GameEngine
{
    public enum TurnState
    {
        PlayerMoving,
        PlayerShooting,
        NpcTurn,
        WaitingForProjectiles,
        InterTurn,
        GameOver
    }

    public enum TurnFaction
    {
        Player,
        Ally,
        Enemy,
        Neutral
    }
}
