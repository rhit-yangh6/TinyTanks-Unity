using System;
using UnityEngine;

namespace _Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager _i;
        private static Chapters _chaptersFromJson;
        
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

            _chaptersFromJson = JsonUtility.FromJson<Chapters>(jsonFile.text);
            
            foreach (var chapter in _chaptersFromJson.chapters)
            {
                chapter.chapterPreviewSprite = Resources.Load<Sprite>("ChapterPreviews/" + chapter.path);
                foreach (var level in chapter.levels)
                {
                    level.levelPreviewSprite = Resources.Load<Sprite>("LevelPreviews/" + level.path);
                }
            }
        }

        public Level GetLevelById(string levelId)
        {
            var chapter = Array.Find(_chaptersFromJson.chapters,
                c => Array.Find(c.levels, l => l.id == levelId) != null);

            return Array.Find(chapter.levels, l => l.id == levelId);
        }

        public Level GetNextLevel(string levelId)
        {
            var chapter = Array.Find(_chaptersFromJson.chapters,
                c => Array.Find(c.levels, l => l.id == levelId) != null);
            var idx = Array.FindIndex(chapter.levels, l => l.id == levelId);
            return idx + 1 >= chapter.levels.Length ? null : chapter.levels[idx + 1];
        }

        public Level[] GetAllLevelsFromChapter(int chapterId)
        {
            var chapter = Array.Find(_chaptersFromJson.chapters,
                c => c.id == chapterId);
            return chapter == null ? Array.Empty<Level>() : chapter.levels;
        }

        public Chapter[] GetAllChapters()
        {
            return _chaptersFromJson.chapters;
        }
    }
    
    [Serializable]
    public class Level
    {
        // Read from Levels.json
        public int prize, weaponPrize;
        public string name, path, id;

        public Sprite levelPreviewSprite;
    }

    [Serializable]
    public class Chapter
    {
        public int id;
        public string name, path;

        public Level[] levels;
        public Sprite chapterPreviewSprite;
    }
        
    [Serializable]
    public class Chapters
    {
        public Chapter[] chapters;
    }
}