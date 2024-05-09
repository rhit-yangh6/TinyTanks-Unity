using System;
using System.Reflection;
using EditorWrapper;
using TerraformingTerrain2d;
using UnityEditor;
using UnityEngine;

namespace TerraformingTerrain2dEditor
{
    public class CreateConfigurationButton : Button
    {
        private readonly TerraformingTerrain2D _terrain;

        public CreateConfigurationButton(string name, TerraformingTerrain2D terrain) : base(name)
        {
            _terrain = terrain;
        }

        protected override void OnClicked()
        {
            string filePath = EditorUtility.SaveFilePanel("Save configuration", "Assets", "Configuration", "asset");

            if (filePath.Length != 0)
            {
                ScalarField scalarField = GetScalarField();
                GridConfiguration configuration = CreateConfiguration(scalarField);
                SaveConfiguration(configuration, filePath);
                InjectConfigurationIntoTerrainComponent(configuration);
            }
        }

        private ScalarField GetScalarField()
        {
            return _terrain.GetByReflection<TerraformingTerrainFacade>("_facade")
                           .GetByReflection<TerraformingTerrainComponents>("_components").ScalarField;
        }

        private GridConfiguration CreateConfiguration(ScalarField scalarField)
        {
            GridConfiguration gridConfiguration = ScriptableObject.CreateInstance<GridConfiguration>();
            gridConfiguration.Initialize(_terrain.Data.SdfTexture, _terrain.Data.SdfFactor, _terrain.Data.Density, scalarField.Value.Array);

            return gridConfiguration;
        }

        private void SaveConfiguration(GridConfiguration configuration, string filePath)
        {
            filePath = "Assets" + filePath.Substring(Application.dataPath.Length);

            AssetDatabase.CreateAsset(configuration, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void InjectConfigurationIntoTerrainComponent(GridConfiguration configuration)
        {
            Type type = typeof(GridConfigurationPresenter);
            FieldInfo preset = type.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            preset?.SetValue(_terrain.Data.Configuration, configuration);
        }
    }
}