using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SearchService;
using System;

public class ChangeShader : EditorWindow
{
    [MenuItem("URP_Adapt/Change Shader")]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = GetWindow(typeof(ChangeShader));
        editorWindow.autoRepaintOnSceneChange = false;
    }
    //当前shader
    public Material currentShader;
    //目标shader
    public Material changeShader;

    private void OnGUI()
    {
        currentShader = EditorGUILayout.ObjectField("查找材质", currentShader, typeof(Material), false) as Material;
        changeShader = EditorGUILayout.ObjectField("以此材质替换", changeShader, typeof(Material), false) as Material;
        if (GUILayout.Button("Change"))
        {
            Change();
        }
    }

    public void Change()
    {
        // 加载的路径需要加后缀名
        if (currentShader == null || changeShader == null)
        {
            return;
        }
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            try
            {
                if (PrefabUtility.IsPartOfPrefabAsset(obj)) continue;
                SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
                if (spr.sharedMaterial.ToString().Equals("Sprites-Default (Instance) (UnityEngine.Material)"))
                {
                    Debug.Log("Changed "+spr.gameObject.name);
                    spr.sharedMaterial = changeShader;
                    Debug.Log("Done");
                }
            }
            catch (Exception)
            {

            }
        }
    }
}