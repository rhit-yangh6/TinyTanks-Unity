using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SDFGeneration
{
#if UNITY_EDITOR
    public static class FileSaveUtils
    {
        public static Texture2D SaveTexture(byte[] rawData)
        {
            string filePath = GetFileName();

            CreateTextureFile(filePath, rawData);
            ConfigureTexture(filePath);

            return (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture));
        }

        private static void CreateTextureFile(string filePath, byte[] rawData)
        {
            File.WriteAllBytes(filePath, rawData);
            AssetDatabase.Refresh();
        }

        private static void ConfigureTexture(string filePath)
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

        private static string GetFileName()
        {
            string filePath = EditorUtility.SaveFilePanel("Save Signed Distance Field",
                "Assets", $"{Guid.NewGuid()} SDF", "png");

            if (filePath.Length == 0)
            {
                throw new Exception("Empty name for texture");
            }

            return filePath;
        }
    }
#endif
}