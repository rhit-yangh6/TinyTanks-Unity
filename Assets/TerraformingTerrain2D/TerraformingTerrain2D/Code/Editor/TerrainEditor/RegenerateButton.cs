using System;
using System.Reflection;
using EditorWrapper;
using TerraformingTerrain2d;

namespace TerraformingTerrain2dEditor
{
    public class RegenerateButton : Button
    {
        private readonly TerraformingTerrain2D _terrain;

        public RegenerateButton(TerraformingTerrain2D terrain) : base("Regenerate")
        {
            _terrain = terrain;
        }

        protected override void OnClicked()
        {
            Type terrainType = typeof(TerraformingTerrain2D);
            MethodInfo regenerateMethod = terrainType.GetMethod("Regenerate", BindingFlags.NonPublic | BindingFlags.Instance);
            regenerateMethod?.Invoke(_terrain, null);
        }
    }
}