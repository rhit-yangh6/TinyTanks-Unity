using UnityEngine;

namespace DemosShared
{
    public static class SceneLoadingProvider
    {
        public static ISceneLoading GetSceneLoading()
        {
            if (Application.isEditor)
            {
                return new EditorSceneLoading();    
            }

            return new InBuildSceneLoading();
        }
    }
}