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
            var jsonFile = Resources.Load<TextAsset>("Data/Levels");

            _levelsFromJson = JsonUtility.FromJson<Levels>(jsonFile.text);
            
            foreach (var level in _levelsFromJson.levels)
            {
                level.levelPreviewSprite = Resources.Load<Sprite>("LevelPreviews/" + level.path);
            }
        }

        public Level GetLevelByPath(string levelPath)
        {
            return Array.Find(_levelsFromJson.levels, l => l.path == levelPath);
        }

        public Level GetNextLevel(string sceneName)
        {
            var currentLevel = Array.Find(_levelsFromJson.levels, l => l.path == sceneName);
            return Array.Find(_levelsFromJson.levels, l => l.id == currentLevel.id + 1);
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

        public Sprite levelPreviewSprite;
    }
        
    [Serializable]
    public class Levels
    {
        public Level[] levels;
    }
}