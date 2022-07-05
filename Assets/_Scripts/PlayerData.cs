using System;
using UnityEngine;

namespace _Scripts
{
    [Serializable]
    public class PlayerData
    {
        public int level;
        public int coins;

        public int[] selectedWeapons;
        public WeaponDatum[] weaponLevels;
        
        private static PlayerData _i;
        
        public static PlayerData Instance
        {
            get
            {
                if (_i == null)
                {
                    PlayerData loadedPlayerData = SaveSystem.LoadPlayer();
                    if (loadedPlayerData == null)
                    {
                        Debug.Log("Generating new player data");
                        loadedPlayerData = new PlayerData();
                    }                    
                    _i = loadedPlayerData;
                }
                return _i;
            }
        }

        public PlayerData()
        {
            /*
             * This is the initial player data
             */
            level = 1;
            coins = 1000;
            selectedWeapons = new int[] { 1, 2, 6, 4, 5 };
            weaponLevels = new WeaponDatum[] { 
                new WeaponDatum(1, 1),
                new WeaponDatum(2, 1), 
                new WeaponDatum(3, 1),
                new WeaponDatum(4, 1), 
                new WeaponDatum(5, 1),
                new WeaponDatum(6, 1)
            };
        }

        public int GetWeaponLevelFromId(int idToFind)
        {
            WeaponDatum datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);
            return datum?.level ?? 0;
        }

        public bool ChangeWeaponSelection(int index, int weaponId)
        {
            int pos = Array.IndexOf(selectedWeapons, weaponId);
            if (pos > -1)
            {
                return false;
            }

            selectedWeapons[index] = weaponId;
            return true;
        }

        public bool SwapWeaponSelection(int index1, int index2)
        {
            (selectedWeapons[index1], selectedWeapons[index2]) = (selectedWeapons[index2], selectedWeapons[index1]);
            return true;
        }
    }

    [Serializable]
    public class WeaponDatum
    {
        public int weaponId;
        public int level;

        public WeaponDatum(int weaponId, int level)
        {
            this.weaponId = weaponId;
            this.level = level;
        }
    }
}
