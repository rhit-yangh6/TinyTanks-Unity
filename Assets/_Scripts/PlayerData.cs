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
            coins = 100;
            selectedWeapons = new [] { 8, 2, 6, 4, 5 };
            weaponLevels = new WeaponDatum[] { 
                new(1, 1, new []{true, true, false, false, false, false}),
                new(2, 1, new []{true, false, false, false, false, false}), 
                new(3, 1, new []{true, false, false, false, false, false}),
                new(4, 1, new []{true, false, false, false, false, false}), 
                new(5, 1, new []{true, false, false, false, false, false}),
                new(6, 1, new []{true, false, false, false, false, false}),
                new(7, 1, new []{true, false, false, false, false, false}),
                new(8, 1, new []{true, false, false, false, false, false})
            };
        }

        public int GetWeaponLevelFromId(int idToFind)
        {
            WeaponDatum datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);
            return datum?.level ?? 0;
        }
        
        public int GetHighestUnlockedLevel(int idToFind)
        {
            WeaponDatum datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);
            
            if (datum.unlocked[4] || datum.unlocked[5])
            {
                return 4;
            }
            
            for (int i = 0; i < datum.unlocked.Length; i++)
            {
                if (!datum.unlocked[i])
                {
                    return i;
                }
            }
            return 5;
        }

        public bool GetIfLevelUnlocked(int idToFind, int selectedLevel)
        {
            WeaponDatum datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);
            return datum.unlocked[selectedLevel - 1];
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

        public bool SetWeaponLevel(int idToFind, int levelToSet)
        {
            var datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);
            if (!datum.unlocked[levelToSet - 1]) return false;
            datum.level = levelToSet;
            return true;

        }

        public bool BuyWeaponUpgrade(int idToFind, int levelToBuy)
        {
            var cost = WeaponManager.Instance.GetWeaponById(idToFind).upgradeInfos[levelToBuy - 2].cost;
            var datum = Array.Find(weaponLevels, w => w.weaponId == idToFind);

            if (cost > coins || datum.unlocked[levelToBuy - 1]) return false;

            coins -= cost;
            datum.unlocked[levelToBuy - 1] = true;
            return true;
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
}
