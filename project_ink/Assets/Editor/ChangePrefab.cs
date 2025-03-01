using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MyEditorWindow : EditorWindow
{
    GameObject newPrefab;
    string dir;
    [MenuItem("Window/Replace")]
    public static void ShowExample()
    {
        MyEditorWindow wnd = GetWindow<MyEditorWindow>();
        wnd.titleContent = new GUIContent("MyEditorWindow");
    }

    void OnGUI(){
        newPrefab=(GameObject)EditorGUILayout.ObjectField("newPrefab", newPrefab, typeof(GameObject), false);
        dir=EditorGUILayout.TextField("directory", dir);
        if(GUILayout.Button("replace")){
            DirectoryInfo dirInfo;
            try{
                dirInfo=new DirectoryInfo(Application.dataPath+'/'+dir);
            } catch(Exception e){
                Debug.LogError($"cannot open {Application.dataPath+'/'+dir}: {e}");
                return;
            }
            string applicationPath=Application.dataPath.Replace('/','\\');
            foreach(FileInfo f in dirInfo.GetFiles()){
                string s=f.FullName.Replace(applicationPath, "Assets");
                if(s.Contains(".meta")) continue;
                ChangePrefab(s);
            }
        }
    }
    void ChangePrefab(string path){
        // 加载 Prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("fail to load prefab at: " + path);
            return;
        }

        // 实例化 Prefab
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        // 找到需要替换的游戏物体
        Transform targetObject = instance.transform.Find("==MustHaveObjects==_1");
        if (targetObject == null) {
            Debug.LogError("cannot find object: ==MustHaveObjects==");
            return;
        }
        if(!targetObject.name.Contains("==MustHaveObjects==")){
            Debug.Log($"prefab {targetObject.name} is already replaced");
            return;
        }

        // 实例化新的游戏物体
        GameObject newInstance = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;

        // 替换游戏物体
        newInstance.transform.SetParent(targetObject.parent);
        newInstance.transform.localScale=Vector3.one;

        // 销毁旧的游戏物体
        DestroyImmediate(targetObject.gameObject);

        // 保存修改后的 Prefab
        PrefabUtility.SaveAsPrefabAsset(instance, path);
        Debug.Log("success with " + path);

        // 销毁实例
        DestroyImmediate(instance);
    }
    /*
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);
        button.clicked+=

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);
    }*/
}