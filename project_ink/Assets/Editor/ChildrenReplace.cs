using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using System.Text;
using System.IO;

public class ChildrenReplace : EditorWindow
{
    GameObject prefab;
    string path;
    [MenuItem("Window/ChildrenReplace")]
    public static void ShowExample()
    {
        ChildrenReplace wnd = GetWindow<ChildrenReplace>();
        wnd.titleContent = new GUIContent("Window");
    }
    void CreateGUI(){
        // Root visual element
        var root = rootVisualElement;

        TextField pathField=new TextField();
        pathField.RegisterValueChangedCallback(PathValueChanged);
        pathField.label="path";
        pathField.value="/Prefabs/MapGeneration";
        root.Add(pathField);
        //object field
        ObjectField targetObjField=new ObjectField();
        targetObjField.objectType=typeof(GameObject);
        targetObjField.value=prefab;
        targetObjField.RegisterValueChangedCallback(ValueChanged);
        root.Add(targetObjField);

        UnityEngine.UIElements.Button button=new UnityEngine.UIElements.Button();
        button.text="test";
        button.clicked+=OnClick;
        root.Add(button);
    }
    //categorize children in tilemap base on their layer (into ground and platform)
    bool ProcessPrefab(GameObject prefab, string prefabPath){
        // Find the Tilemap GameObject
        Transform tilemap = prefab.transform.Find("Tilemap");
        if(tilemap==null)
            tilemap=prefab.transform.Find("tilemap");
        if (tilemap == null)
        {
            Debug.LogWarning($"Tilemap not found in prefab: {prefabPath}");
            return false;
        }
        CompositeCollider2D tc=tilemap.GetComponent<CompositeCollider2D>();
        if(tc==null){
            return false;
        }
        tc.geometryType=CompositeCollider2D.GeometryType.Polygons;

        return true;
    }
    void ProcessPrefabsInFile(){
        string file=@"D:\Wensi_Xie\Github\project_ink\project_ink\prefabs_fullpath.txt";
        StreamReader reader=new StreamReader(file);
        while(!reader.EndOfStream){
            string input=reader.ReadLine();
            GameObject prefab = PrefabUtility.LoadPrefabContents(input);

            if(ProcessPrefab(prefab, input)){
                PrefabUtility.SaveAsPrefabAsset(prefab, input);
                Debug.Log($"Successfully Processed prefab: {input}");
            }

            // Save the modified prefab
            PrefabUtility.UnloadPrefabContents(prefab);
        }
    }
    private void ProcessPrefabsInDirectory()
    {
        // Get all prefab files in the directory
        string fullPath=Application.dataPath+path;
        string[] prefabPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string prefabPath in prefabPaths)
        {
            // Load the prefab
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);

            bool success=ProcessPrefab(prefab, prefabPath);

            // Save the modified prefab
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefab);
            if(success)
                Debug.Log($"Processed prefab: {prefabPath}");
            else
                Debug.LogWarning($"Failed to process prefab: {prefabPath}");
        }

        Debug.Log("Prefab processing complete!");
    }
    void ValueChanged(ChangeEvent<Object> evt){
        prefab=evt.newValue as GameObject;
    }
    void PathValueChanged(ChangeEvent<string> evt){
        path=evt.newValue;
    }
    void OnClick(){
        ProcessPrefabsInDirectory();
    }
}