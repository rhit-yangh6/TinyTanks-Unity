using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _Scripts
{
    public static class SaveSystem
    {
        public static void SavePlayer()
        {
            Debug.Log("save!");
            PlayerData dataToSave = PlayerData.Instance;

            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.data";
            
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, dataToSave);
            
            stream.Close();
        }

        public static PlayerData LoadPlayer()
        {
            string path = Application.persistentDataPath + "/player.data";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                
                PlayerData data = formatter.Deserialize(stream) as PlayerData;

                stream.Close();
                
                return data;
            }

            return null;
        }
    }
}
