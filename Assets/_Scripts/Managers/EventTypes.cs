namespace _Scripts.Managers
{
    public enum EventTypes
    {
        /* Represents the event that game is paused */
        PauseGame,
        
        /* Represents the event that game is resumed */
        ResumeGame,
        
        /* Represents the event that an Entity has taken some damage */
        DamageDealt,
        
        /* Represents the event that terrain needs to be destroyed */
        DestroyTerrain,
        
        /* Represents the event that an Projectile is shot from player/enemy */
        ProjectileShot,
        
        /* Represents the event that player started dragging */
        StartedDragging,
        
        /* Represents the event that player stopped Dragging */
        StoppedDragging,
        
        /* Represents the event that a turn is ended by either player/enemy */
        EndTurn,
        
        /* Represents the change of state of Discord Rich Presence */
        DiscordStateChange
    }
}