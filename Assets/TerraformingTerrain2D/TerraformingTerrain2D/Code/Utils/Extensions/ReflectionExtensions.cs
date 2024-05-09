using System;
using System.Reflection;

namespace TerraformingTerrain2d
{
    public static class ReflectionExtensions
    {
        public static T GetByReflection<T>(this object instance, string name)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            
            return (T)fieldInfo?.GetValue(instance);
        }
        
        public static T GetByReflectionRecursive<T>(this object instance, string name, int depth = 3)
        {
            return GetRecursive<T>(instance, name, -1, depth);
        }

        private static T GetRecursive<T>(object instance, string name, int depthLevel, int depth)
        {
            if (++depthLevel >= depth)
                return default;
            
            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; ++i)
            {
                object fieldValue = fields[i].GetValue(instance);
                
                if (fields[i].Name == name)
                {
                    return (T)fieldValue;
                }

                GetRecursive<T>(fieldValue, name, depthLevel, depth);
            }
            
            return default;
        }
    }   
}