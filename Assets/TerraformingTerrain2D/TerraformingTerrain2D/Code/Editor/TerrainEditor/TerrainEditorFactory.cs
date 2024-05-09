using EditorWrapper;
using TerraformingTerrain2d;
using UnityEditor;

namespace TerraformingTerrain2dEditor
{
    public class TerrainEditorFactory
    {
        private readonly SerializedObject _serializedObject;
        private readonly TerraformingTerrain2D _terrain;
        private readonly int _space = 10;

        public TerrainEditorFactory(SerializedObject serializedObject, TerraformingTerrain2D terrain)
        {
            _serializedObject = serializedObject;
            _terrain = terrain;
        }

        private SerializedProperty Data => _serializedObject.FindProperty("_data"); 
        private SerializedProperty Input => Data.FindPropertyRelative("_input"); 
        private SerializedProperty Configuration => Data.FindPropertyRelative("_configuration"); 
        private SerializedProperty MultiThreading => Data.FindPropertyRelative("_multithreading"); 
        
        public IDrawable Create()
        {
            IDrawable initializationSection = CreateInitializationSection();
            IDrawable editor = new DrawableComposite(new[]
            {
                CreateСhunkSection(),
                CreateConfigurationSection(),
                CreateGridSection(),
                CreateViewSection(),
                CreateOtherSection()
            });

            return new ConditionalDraw(editor, initializationSection, () => _terrain.Data.Input.CheckIfValid());
        }

        private IDrawable CreateInitializationSection()
        {
            return new Section("Initialization stage", 0, new IDrawable[]
            {
                new Property(Input.FindPropertyRelative("_sdfTexture")),
                new Property(Input.FindPropertyRelative("_material"))
            });
        }

        private IDrawable CreateСhunkSection()
        {
            return new Section("Chunks", _space, new IDrawable[]
            {
                new RegenerateButton(_terrain),
                new Property(Data.FindPropertyRelative("_chunkCountX")),
                new Property(Data.FindPropertyRelative("_chunkCountY")),
                new ReadOnlyElement(new Property(Data.FindPropertyRelative("_generatedChunksCount"))),
            });
        }

        private IDrawable CreateGridSection()
        {
            IDrawable section = new Section("Grid", _space, new IDrawable[]
            {
                new Property(Input.FindPropertyRelative("_sdfTexture")),
                new Property(Data.FindPropertyRelative("_sdfFactor")),
                new Property(Data.FindPropertyRelative("_density")),
            });

            return new HideIfDrawable(section, () => _terrain.Data.Configuration.IsInUse);
        }

        private IDrawable CreateConfigurationSection()
        {
            return new Section("Configuration", _space, new IDrawable[]
            {
                new CreateConfigurationButton("Save configuration", _terrain),
                new Property(Configuration.FindPropertyRelative("_preset")),
            });
        }

        private IDrawable CreateViewSection()
        {
            return new Section("View", _space, new IDrawable[]
            {
                new Property(Data.FindPropertyRelative("_scale")),
                new Property(Data.FindPropertyRelative("_sortingOrder")),
                new Property(Input.FindPropertyRelative("_material")),
            });
        }

        private IDrawable CreateOtherSection()
        {
            return new Section("Other", 0, new IDrawable[]
            {
                new Property(MultiThreading.FindPropertyRelative("_useMultithreading")),
                //new Property(data.FindPropertyRelative("_gizmoData")),
            });
        }
    }
}