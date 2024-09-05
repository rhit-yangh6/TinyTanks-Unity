using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.GameEngine
{
    [Serializable]
    public class PlayerData
    {
        public Dictionary<int, int> Levels;
        public HashSet<string> PassedLevels, DiscoveredMusic;
        public HashSet<int> CheckedWeapons;
        public int coins;

        public SelectionDatum[] selectedWeapons;
        public List<WeaponDatum> weaponLevels;

        public bool isTutorialCompleted;
        
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
            
            // Deprecated
            
            Levels = new Dictionary<int, int> { { 1, 0 } };
            PassedLevels = new HashSet<string>();
            DiscoveredMusic = new HashSet<string>();
            CheckedWeapons = new HashSet<int>();

            coins = 88888888;
            isTutorialCompleted = false;
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
                new(-1, 1, new[]{true, true, true, true, true, false}), // TODO: Delete!
                new(1, 1, new[]{true, false, false, false, false, false}),
                new(2, 1, new[]{true, false, false, false, false, false})
            };

            // Uncomment this if you want everything unlocked
            // var datumList = new List<WeaponDatum>();
            // datumList.AddRange(WeaponManager.Instance.GetAllWeapons().Select(w => new WeaponDatum(w.id, 1, new[] { true, true, true, true, true, true })));
            // weaponLevels = datumList;
        }

        public int GetLevelStatusInChapter(int chapterId)
        {
            if (Levels.TryAdd(chapterId, 0))
            {
                SaveSystem.SavePlayer();
                return 0;
            }

            return Levels[chapterId];
        }

        public bool IsLevelPassed(string levelId)
        {
            return PassedLevels.Contains(levelId);
        }

        public bool IsChapterUnlocked(int chapterId)
        {
            return Levels.ContainsKey(chapterId);
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
            SaveSystem.SavePlayer();
            return true;
        }

        public bool SwapWeaponSelection(int index1, int index2)
        {
            // Illegal Swap
            if ((index1 == 0 || index2 == 0) && 
                (selectedWeapons[index1] == null || selectedWeapons[index2] == null)) return false;

            (selectedWeapons[index1], selectedWeapons[index2]) = (selectedWeapons[index2], selectedWeapons[index1]);
            SaveSystem.SavePlayer();
            return true;
        }
        
        public bool ClearWeaponSelection(int weaponId)
        {
            // If this is the only weapon selected
            if (selectedWeapons.Count(sw => sw == null) == 4)
            {
                return false;
            }
            
            // Illegal Clear
            var idx = Array.FindIndex(selectedWeapons, sw => sw != null && sw.weaponId == weaponId);
            if (idx == -1) return false;

            selectedWeapons[idx] = null;
            SaveSystem.SavePlayer();
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

            SaveSystem.SavePlayer();
            return true;
        }

        public bool BuyWeaponUpgrade(int idToFind, int levelToBuy)
        {
            var cost = WeaponManager.Instance.GetWeaponById(idToFind).upgradeInfos[levelToBuy - 2].cost;
            var datum = weaponLevels.Find(wl => wl.weaponId == idToFind);

            if (cost > coins || datum.unlocked[levelToBuy - 1]) return false;

            coins -= cost;
            datum.unlocked[levelToBuy - 1] = true;
            EventBus.Broadcast(EventTypes.CoinChanged);
            SaveSystem.SavePlayer();
            return true;
        }

        public bool BuyWeapon(int idToFind)
        {
            var cost = WeaponManager.Instance.GetWeaponById(idToFind).shopPrice;

            if (cost > coins || GetWeaponLevelFromId(idToFind) > 0) return false;

            coins -= cost;
            weaponLevels.Add(new WeaponDatum(idToFind, 1, new []{true, false, false, false, false, false}));
            EventBus.Broadcast(EventTypes.CoinChanged);
            SaveSystem.SavePlayer();
            return true;
        }
        
        public bool SpendCoins(int amount)
        {
            // Spend Coins from other sources (weapons)

            if (amount > coins) return false;

            coins -= amount;
            EventBus.Broadcast(EventTypes.CoinChanged);
            SaveSystem.SavePlayer();
            return true;
        }

        public void CompleteLevel()
        {
            var chapter = Array.Find(LevelManager.Instance.GetAllChapters(),
                c => c.id == GameStateController.currentChapterId);
            
            var currentIdx = Array.FindIndex(chapter.levels, l => l.id == GameStateController.currentLevelId);
            var currentLevel = chapter.levels[currentIdx];
            var progress = GetLevelStatusInChapter(GameStateController.currentChapterId);

            PassedLevels.Add(currentLevel.id);
            
            // First completion
            if (progress <= currentIdx && currentIdx < chapter.levels.Length - 1)
            {
                Levels[GameStateController.currentChapterId] = currentIdx + 1;
            }
            // If the level unlocks a chapter
            else if (currentLevel.unlocksChapter != 0)
            {
                UnlockChapter(currentLevel.unlocksChapter);
            }
            SaveSystem.SavePlayer();
        }

        public void UnlockChapter(int chapterId)
        {
            if (Levels.ContainsKey(chapterId))
            {
                return;
            }
            Levels.Add(chapterId, 0);
            SaveSystem.SavePlayer();
        }

        public void GainMoney(int prize)
        {
            coins += prize;
            var gainedAmount = SteamManager.IncrementStat(Constants.StatCoinsGained, prize);
            if (gainedAmount > 2500)
            {
                SteamManager.UnlockAchievement(Constants.AchievementEarnedABit);
                WeaponManager.UnlockWeapon(35); // Piggy Bank 35
            }
            EventBus.Broadcast(EventTypes.CoinChanged);
            SaveSystem.SavePlayer();
        }

        public void CompleteTutorial()
        {
            isTutorialCompleted = true;
            SaveSystem.SavePlayer();
        }

        public bool IsWeaponChecked(int weaponId)
        {
            return CheckedWeapons.Contains(weaponId);
        }
        
        public void CheckWeapon(int weaponId)
        {
            if (!CheckedWeapons.Add(weaponId))
            {
                return;
            }

            SaveSystem.SavePlayer();
        }

        public int WeaponsSelected()
        {
            return selectedWeapons.Count(sw => sw != null);
        }

        public void DiscoverMusic(string musicName)
        {
            DiscoveredMusic.Add(musicName);
            if (DiscoveredMusic.Count >= 5)
            {
                SteamManager.UnlockAchievement(Constants.AchievementAudiophile);
                WeaponManager.UnlockWeapon(22); // Boombox 22
            }
            
            Debug.Log(DiscoveredMusic.ToString());
            SaveSystem.SavePlayer();
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
