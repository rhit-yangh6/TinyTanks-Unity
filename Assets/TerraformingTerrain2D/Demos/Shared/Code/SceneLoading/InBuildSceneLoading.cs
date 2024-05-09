using UnityEngine.SceneManagement;

namespace DemosShared
{
    public class InBuildSceneLoading : ISceneLoading
    {
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }    
}
