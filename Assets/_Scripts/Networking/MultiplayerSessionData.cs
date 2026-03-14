using Steamworks;

namespace _Scripts.Networking
{
    /// <summary>
    /// Static class holding multiplayer session state.
    /// Populated from lobby metadata before match start.
    /// </summary>
    public static class MultiplayerSessionData
    {
        public static bool IsMultiplayer;
        public static bool IsHost;
        public static bool IsBotMode;
        public static int LocalPlayerIndex;
        public static int PlayerCount;

        public static SteamId[] PlayerSteamIds = new SteamId[4];
        public static string[] PlayerNames = new string[4];

        /// <summary>
        /// Each player's weapon loadout: PlayerWeapons[playerIndex][slotIndex] = (weaponId, level)
        /// </summary>
        public static (int weaponId, int level)[][] PlayerWeapons = new (int, int)[4][];

        public static void Clear()
        {
            IsMultiplayer = false;
            IsHost = false;
            IsBotMode = false;
            LocalPlayerIndex = 0;
            PlayerCount = 0;
            PlayerSteamIds = new SteamId[4];
            PlayerNames = new string[4];
            PlayerWeapons = new (int, int)[4][];
        }

        /// <summary>
        /// Parse weapon loadout string from lobby metadata.
        /// Format: "weaponId:level,weaponId:level,..."
        /// </summary>
        public static (int weaponId, int level)[] ParseWeaponLoadout(string data)
        {
            if (string.IsNullOrEmpty(data)) return System.Array.Empty<(int, int)>();

            var entries = data.Split(',');
            var result = new (int weaponId, int level)[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                var parts = entries[i].Split(':');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int weaponId) &&
                    int.TryParse(parts[1], out int level))
                {
                    result[i] = (weaponId, level);
                }
            }

            return result;
        }

        /// <summary>
        /// Serialize weapon loadout to string for lobby metadata.
        /// </summary>
        public static string SerializeWeaponLoadout((int weaponId, int level)[] weapons)
        {
            var parts = new string[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                parts[i] = $"{weapons[i].weaponId}:{weapons[i].level}";
            }

            return string.Join(",", parts);
        }
    }
}
