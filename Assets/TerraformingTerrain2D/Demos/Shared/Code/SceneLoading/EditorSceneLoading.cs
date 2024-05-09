#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace DemosShared
{
    public class EditorSceneLoading : ISceneLoading
    {
        public void Load(string sceneName)
        {
            string[] paths = AssetDatabase.FindAssets($"{sceneName} t:Scene");
            string path = AssetDatabase.GUIDToAssetPath(paths[0]);

            EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters());
        }
    }
}
#endif