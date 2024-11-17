using System;
using System.Reflection;
using Combat;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class MenuScripts
    {
        [MenuItem("Assets/Create/Game/New Unit")]
        private static void NewUnit()
        {
            AssetDatabase.CopyAsset("Assets/Resources/Units/Unit Base.prefab", $"{CurrentFolder()}/New Unit.prefab");
        }

        private static string CurrentFolder()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }
    }
}