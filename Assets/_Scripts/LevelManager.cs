using System;
using UnityEngine;

namespace _Scripts
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager _i;
        private static Levels _levelsFromJson;
        
        public static LevelManager Instance
        {
            get
            {
                if (_i == null)
                {
                    _i = Instantiate(Resources.Load<LevelManager>("LevelManager"));
                    LoadLevels();
                }
                return _i;
            }
        }
        
        private static void LoadLevels()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("Data/Levels");

            _levelsFromJson = JsonUtility.FromJson<Levels>(jsonFile.text);
        }

        public Level GetLevelById(int idToFind)
        {
            return Array.Find(_levelsFromJson.levels, l => l.id == idToFind); 
        }

        public Level[] GetAllLevels()
        {
            return _levelsFromJson.levels;
        }
    }
    
    [Serializable]
    public class Level
    {
        // Read from Levels.json
        public int id, prize;
        public string name, path;

    }
        
    [Serializable]
    public class Levels
    {
        public Level[] levels;
    }
}