using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public static class SaveSystem
    {
        public static void SavePlayer()
        {
            Debug.Log("Saving Player Data...");
            PlayerData dataToSave = PlayerData.Instance;

            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.data";
            
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, dataToSave);
            
            stream.Close();
        }

        public static PlayerData LoadPlayer()
        {
            Debug.Log("Loading Player Data...");
            var path = Application.persistentDataPath + "/player.data";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                
                var data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                if (data == null) return null;
                
                data.PassedLevels ??= new HashSet<string>();
                data.CheckedWeapons ??= new HashSet<int>();
                data.Levels ??= new Dictionary<int, int> { { 1, 0 } };;
                return data;
            }

            return null;
        }
    }
}
