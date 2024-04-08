# if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kuchinashi
{
    public class EditorExtension : EditorWindow
    {
        [MenuItem ("Kuchinashi/Open Persistent Data Folder")]
        public static void OpenPersistentDataFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath, Application.productName));
        }

        [MenuItem ("Kuchinashi/Delete Persistent Data Folder")]
        public static void DeletePersistentDataFolder()
        {
            Directory.Delete(Application.persistentDataPath, true);
        }
    }

}

#endif