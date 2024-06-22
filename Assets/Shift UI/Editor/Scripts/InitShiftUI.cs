using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Shift
{
    public class InitShiftUI : MonoBehaviour
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("ShiftUI.HasCustomEditorData"))
                {
                    EditorPrefs.SetInt("ShiftUI.HasCustomEditorData", 1);

                    string mainPath = AssetDatabase.GetAssetPath(Resources.Load("Shift UI Manager"));
                    mainPath = mainPath.Replace("Resources/Shift UI Manager.asset", "").Trim();
                    string darkPath = mainPath + "Editor/Shift UI Skin Dark.guiskin";
                    string lightPath = mainPath + "Editor/Shift UI Skin Light.guiskin";

                    EditorPrefs.SetString("ShiftUI.CustomEditorDark", darkPath);
                    EditorPrefs.SetString("ShiftUI.CustomEditorLight", lightPath);
                }
            }
        }
    }
}