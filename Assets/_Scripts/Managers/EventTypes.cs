namespace _Scripts.Managers
{
    public enum EventTypes
    {
        /* Represents the event that an Entity has taken some damage */
        DamageDealt,
        
        /* Represents the event that an Projectile is shot from player/enemy */
        ProjectileShot,
        
        /* Represents the event that a turn is ended by either player/enemy */
        EndTurn,
        
        /* Represents the change of state of Discord Rich Presence */
        DiscordStateChange
    }
}