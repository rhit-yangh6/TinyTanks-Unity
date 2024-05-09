using System;
using System.IO;
using CatlikeCoding.SDFToolkit;
using UnityEditor;
using UnityEngine;

namespace SDFGeneration
{
#if UNITY_EDITOR
    public class SDFTextureFromImageGenerator : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture;

        public void Setup()
        {
            if (_texture == null)
            {
                Debug.LogError("Set texture for terrain generation");
                return;
            }

            ConfigureSourceTexture();
            Texture2D destination = new(_texture.width, _texture.height);
            SDFTextureGenerator.Generate(_texture, destination, 30, 0, 0, RGBFillMode.Distance);
            SaveTexture(destination);
        }

        private void ConfigureSourceTexture()
        {
            string path = AssetDatabase.GetAssetPath(_texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("Cannot work with built-in textures.");
                return;
            }

            if (importer.crunchedCompression)
            {
                Debug.LogError("You have to disable crunch compression while generating the SDF texture.");
                return;
            }

            bool isReadble = importer.isReadable;
            TextureImporterCompression compression = importer.textureCompression;

            bool uncompressed = compression == TextureImporterCompression.Uncompressed;

            if (!isReadble || !uncompressed)
            {
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                AssetDatabase.ImportAsset(path);
            }
        }

        private Texture2D SaveTexture(Texture2D destination)
        {
            string filePath = GetFileName();
            bool isNewTexture = File.Exists(filePath) == false;

            CreateTextureFile(filePath, destination);
            ConfigureTexture(isNewTexture, filePath);

            string sourcePath = AssetDatabase.GetAssetPath(_texture);
            string destinationPath = sourcePath.Replace(_texture.name, _texture.name + " SDF");

            return (Texture2D)AssetDatabase.LoadAssetAtPath(destinationPath, typeof(Texture));
        }

        private string GetFileName()
        {
            string filePath = EditorUtility.SaveFilePanel("Save Signed Distance Field",
                new FileInfo(AssetDatabase.GetAssetPath(_texture)).DirectoryName,
                _texture.name + " SDF",
                "png");

            if (filePath.Length == 0)
            {
                throw new Exception("Empty name for texture");
            }

            return filePath;
        }

        private void CreateTextureFile(string filePath, Texture2D destination)
        {
            File.WriteAllBytes(filePath, destination.EncodeToPNG());
            AssetDatabase.Refresh();
        }

        private void ConfigureTexture(bool isNewTexture, string filePath)
        {
            if (isNewTexture)
            {
                int relativeIndex = filePath.IndexOf("Assets/", StringComparison.Ordinal);
                if (relativeIndex >= 0)
                {
                    filePath = filePath.Substring(relativeIndex);
                    TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.SingleChannel;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.isReadable = true;
                        importer.npotScale = TextureImporterNPOTScale.None;
                        AssetDatabase.ImportAsset(filePath);
                        return;
                    }
                }

                Debug.LogWarning(
                    "Failed to setup exported texture as uncompressed single channel. You have to configure it manually.");
            }
        }
    }
#endif
}