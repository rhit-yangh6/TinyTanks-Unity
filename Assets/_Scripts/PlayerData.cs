using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    [Serializable]
    public class PlayerData
    {
        public int level;
        public int coins;

        public SelectionDatum[] selectedWeapons;
        public List<WeaponDatum> weaponLevels;
        
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
            coins = 88888888;
            selectedWeapons = new SelectionDatum[]
            {
                new (1, 1),
                new (2, 1),
                null,
                null,
                null
            };
            weaponLevels = new List<WeaponDatum>
            {
                new(1, 1, new []{true, false, false, false, false, false}),
                new(2, 1, new []{true, false, false, false, false, false})
            };

        }

        public int GetWeaponLevelFromId(int idToFind)
        {
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);
            return datum?.level ?? 0;
        }
        
        public int GetHighestUnlockedLevel(int idToFind)
        {
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);
            
            if (datum.unlocked[4] || datum.unlocked[5]) return 4;

            for (int i = 0; i < datum.unlocked.Length; i++)
            {
                if (!datum.unlocked[i]) return i;
            }
            return 5;
        }

        public bool GetIfLevelUnlocked(int idToFind, int selectedLevel)
        {
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);
            return datum.unlocked[selectedLevel - 1];
        }
        
        public bool ChangeWeaponSelection(int index, int weaponId)
        {
            var sd = Array.Find(selectedWeapons, sd => sd != null && sd.weaponId == weaponId);
            if (sd != null) return false;

            selectedWeapons[index] = new SelectionDatum(weaponId, GetWeaponLevelFromId(weaponId));
            return true;
        }

        public bool SwapWeaponSelection(int index1, int index2)
        {
            // Illegal Swap
            if ((index1 == 0 || index2 == 0) && 
                (selectedWeapons[index1] == null || selectedWeapons[index2] == null)) return false;

            (selectedWeapons[index1], selectedWeapons[index2]) = (selectedWeapons[index2], selectedWeapons[index1]);
            return true;
        }

        public bool SetWeaponLevel(int idToFind, int levelToSet)
        {
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);
            if (!datum.unlocked[levelToSet - 1]) return false;

            // Set the level
            datum.level = levelToSet;
            
            // Set the level if the weapon is selected
            var sd = Array.Find(selectedWeapons, sd => sd != null && sd.weaponId == idToFind);
            if (sd != null) sd.level = levelToSet;

            return true;
        }

        public bool BuyWeaponUpgrade(int idToFind, int levelToBuy)
        {
            var cost = WeaponManager.Instance.GetWeaponById(idToFind).upgradeInfos[levelToBuy - 2].cost;
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);

            if (cost > coins || datum.unlocked[levelToBuy - 1]) return false;

            coins -= cost;
            datum.unlocked[levelToBuy - 1] = true;
            return true;
        }

        public bool BuyWeapon(int idToFind)
        {
            var cost = WeaponManager.Instance.GetWeaponById(idToFind).shopPrice;

            if (cost > coins || GetWeaponLevelFromId(idToFind) > 0) return false;

            coins -= cost;
            weaponLevels.Add(new WeaponDatum(idToFind, 1, new []{true, false, false, false, false, false}));
            return true;
        }
        
        public void GainMoney(int prize)
        {
            coins += prize;
        }
    }

    [Serializable]
    public class WeaponDatum
    {
        public int weaponId;
        public int level;
        public bool[] unlocked;

        public WeaponDatum(int weaponId, int level, bool[] unlocked)
        {
            this.weaponId = weaponId;
            this.level = level;
            this.unlocked = unlocked;
        }
    }

    [Serializable]
    public class SelectionDatum
    {
        public int weaponId;
        public int level;

        public SelectionDatum(int weaponId, int level)
        {
            this.weaponId = weaponId;
            this.level = level;
        }
    }
}
