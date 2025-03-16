using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using System.Text;
using System.IO;

public class RoomAssignWindow : EditorWindow
{
    StringBuilder sb;
    GameObject prefab;
    Tilemap tilemap;
    RoomSceneConfig config;
    RoomType searchForType;
    int w=24,h=13;
    string path;
    void Init(){
        if(sb==null){
            sb=new StringBuilder();
        }
        sb.Clear();
    }
    private void Log(string msg){
        //for(int i=0;i<inst.indent;++i)
            //inst.sb.Append("    ");
        sb.Append(msg);
        sb.Append('\n');
    }
    public void Write()
    {
        Debug.Log(sb);
        return;
    }
    [MenuItem("Window/RoomAssign")]
    public static void ShowExample()
    {
        RoomAssignWindow wnd = GetWindow<RoomAssignWindow>();
        wnd.titleContent = new GUIContent("Window");
    }
    Transform FindChild(Transform p, string child, int depth){
        Transform ret=p.Find(child);
        if(depth==1||ret!=null) return ret;
        depth--;
        foreach(Transform c in p){
            ret=FindChild(c, child, depth);
            if(ret!=null) return ret;
        }
        return ret;
    }
    static Dictionary<Vector2Int,int> GetCells(Tilemap tilemap, out Vector2Int min, out Vector2Int max){
        Dictionary<Vector2Int,int> cells=new Dictionary<Vector2Int, int>(tilemap.transform.childCount/2);
        min=new Vector2Int(int.MaxValue,int.MaxValue);
        max=new Vector2Int(int.MinValue,int.MinValue);
        Transform ground=tilemap.transform.Find("Ground");
        Transform platform=tilemap.transform.Find("Platform");
        if(ground==null || platform==null){
            Debug.LogError("cannot find ground or platform in Gameobject Tilemap");
            return null;
        }
        foreach(Transform child in ground){
            Vector3 cellPos3=child.localPosition;
            Vector2Int cellPos=new Vector2Int((int)Mathf.Round(cellPos3.x),(int)Mathf.Round(cellPos3.y));
            if(cells.ContainsKey(cellPos)) continue;
            cells.Add(cellPos,1);
            min.x=Mathf.Min(min.x,cellPos.x);
            max.x=Mathf.Max(max.x,cellPos.x);
            min.y=Mathf.Min(min.y, cellPos.y);
            max.y=Mathf.Max(max.y, cellPos.y);
        }
        foreach(Transform child in platform){
            Vector3 cellPos3=child.localPosition;
            Vector2Int cellPos=new Vector2Int((int)Mathf.Round(cellPos3.x),(int)Mathf.Round(cellPos3.y));
            if(cells.ContainsKey(cellPos)) continue;
            cells.Add(cellPos,1);
            min.x=Mathf.Min(min.x,cellPos.x);
            max.x=Mathf.Max(max.x,cellPos.x);
            min.y=Mathf.Min(min.y, cellPos.y);
            max.y=Mathf.Max(max.y, cellPos.y);
        }
        return cells;
    }
    RoomType GetType(GameObject go){
        Transform tilemap_transform=FindChild(go.transform,"Tilemap", 3);
        if(tilemap_transform==null)
            tilemap_transform=FindChild(go.transform, "tilemap", 3);
        if(tilemap_transform==null){
            Debug.LogError("cannot find the tilemap in "+go.name);
            return RoomType.Error;
        }
        Tilemap tilemap=tilemap_transform.GetComponent<Tilemap>();
        if(tilemap==null){
            Debug.LogError("cannot find the tilemap component in "+tilemap_transform.name);
            return RoomType.Error;
        }
        Vector2Int min,max;
        Dictionary<Vector2Int,int> cells=GetCells(tilemap,out min,out max);
        if(cells==null) return RoomType.Error;
        int roomWidth=max.x-min.x>w?2:1;
        int roomHeight=max.y-min.y>h?2:1;
        int doorU=0,doorR=0,doorD=0,doorL=0;
        int doorPos=-1;
        int i,j;
        //doorU
        for(i=min.x,j=max.y;i<=max.x;++i){
            if(!cells.ContainsKey(new Vector2Int(i,j))){
                doorPos=i-min.x;
                doorU=doorPos>=w?2:1;
                break;
            }
        }
        //doorD
        for(i=max.x,j=min.y;i>=min.x;--i){
            if(!cells.ContainsKey(new Vector2Int(i,j))){
                doorPos=max.x-i;
                doorD=doorPos<=w?1:2;
                break;
            }
        }
        //doorR
        for(i=max.x,j=max.y;j>=min.y;--j){
            if(!cells.ContainsKey(new Vector2Int(i,j))){
                doorPos=max.y-j;
                doorR=doorPos<=h?1:2;
                break;
            }
        }
        //doorL
        for(i=min.x,j=min.y;j<=max.y;++j){
            if(!cells.ContainsKey(new Vector2Int(i,j))){
                doorPos=j-min.y;
                doorL=doorPos>=h?2:1;
                break;
            }
        }
        int roomType=0;
        roomType=(roomWidth<<15)|(roomHeight<<12)|(doorU<<9)|(doorR<<6)|(doorD<<3)|doorL;
        return (RoomType)roomType;
    }
    bool AddPrefabToConfig(GameObject prefab){
        if(config==null){
            Debug.LogError("you need to assign config");
            return false;
        }
        if(prefab==null){
            Debug.LogError("you need to assign a room prefab");
            return false;
        }
        RoomType type=GetType(prefab);
        if(type==RoomType.Error){
            Debug.LogError("room type is RoomType.Error. stopped");
            return false;
        }
        int i=0;
        foreach(RoomSceneConfig.Element e in config.roomPrefabs_arrayList){
            //is the target type
            if(e.roomType==type){
                foreach(GameObject go in e.prefabs){
                    if(go==prefab){
                        Debug.LogError($"already contains {prefab.name} in index {i}.");
                        return false;
                    }
                }
                e.prefabs.Add(prefab);
                Debug.Log($"{prefab.name} added in index {i} as type {type}");
                return true;
            }
            ++i;
        }
        config.roomPrefabs_arrayList.Add(new RoomSceneConfig.Element(type, prefab));
        Debug.Log($"{prefab.name} added in index {i} as type {type}");
        return true;
    }
    private void ProcessPrefabsInDirectory()
    {
        // Get all prefab files in the directory
        string fullPath=path;
        Debug.Log($"full path={fullPath}");
        string[] prefabPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string prefabPath in prefabPaths)
        {
            // Load the prefab
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if(prefab==null){
                Debug.LogError($"cannot find prefab at path {prefabPath}");
                break;
            }

            AddPrefabToConfig(prefab);
        }

        Debug.Log("Prefab processing complete!");
    }
    void CreateGUI(){
        // Root visual element
        var root = rootVisualElement;

        //config
        ObjectField configField=new ObjectField();
        configField.objectType=typeof(RoomSceneConfig);
        configField.value=prefab;
        configField.RegisterValueChangedCallback(ConfigValueChanged);
        configField.label="config";
        root.Add(configField);
        //path field
        TextField pathField=new TextField();
        pathField.label="directory";
        pathField.RegisterValueChangedCallback(PathValueChanged);
        pathField.value="/Prefabs/MapGeneration";
        path="/Prefabs/MapGeneration";
        pathField.SetEnabled(false);
        root.Add(pathField);
        //width and height
        IntegerField wField=new IntegerField();
        wField.label="w";
        wField.RegisterValueChangedCallback(WidthValueChanged);
        wField.value=24;
        root.Add(wField);
        IntegerField hField=new IntegerField();
        hField.value=13;
        hField.label="h";
        hField.RegisterValueChangedCallback(HeightValueChanged);
        root.Add(hField);
        //prefab
        ObjectField targetObjField=new ObjectField();
        targetObjField.objectType=typeof(GameObject);
        targetObjField.value=prefab;
        targetObjField.RegisterValueChangedCallback(ValueChanged);
        targetObjField.label="room";
        root.Add(targetObjField);

        //buttons
        UnityEngine.UIElements.Button button=new UnityEngine.UIElements.Button();
        button.text="Add Prefab";
        button.clicked+=OnClick;
        root.Add(button);

        button=new UnityEngine.UIElements.Button();
        button.text="Print Type";
        button.clicked+=PrintTypeOnClick;
        root.Add(button);

        button=new UnityEngine.UIElements.Button();
        button.text="Add Prefabs in directory";
        button.clicked+=AddPrefabsInDirectoryOnClick;
        button.SetEnabled(false);
        root.Add(button);

        //search for the index of a type
        EnumField typeField=new EnumField();
        typeField.Init(RoomType.Error);
        typeField.RegisterValueChangedCallback(TypeValueChanged);
        typeField.label="Search For Type";
        root.Add(typeField);

        button=new UnityEngine.UIElements.Button();
        button.text="Search";
        button.clicked+=SearchTypeOnClick;
        root.Add(button);
    }
    void TypeValueChanged(ChangeEvent<System.Enum> evt){
        searchForType=(RoomType)evt.newValue;
    }
    void PrintTypeOnClick(){
        Debug.Log("type="+GetType(prefab));
    }
    void SearchTypeOnClick(){
        int i=0;
        foreach(RoomSceneConfig.Element e in config.roomPrefabs_arrayList){
            //is the target type
            if(e.roomType==searchForType){
                Debug.Log($"type {searchForType} at index {i}");
                return;
            }
            i++;
        }
        Debug.Log($"did not find type {searchForType}");
    }
    void ValueChanged(ChangeEvent<Object> evt){
        prefab=evt.newValue as GameObject;
    }
    void PathValueChanged(ChangeEvent<string> evt){
        path=evt.newValue;
    }
    void ConfigValueChanged(ChangeEvent<Object> evt){
        config=evt.newValue as RoomSceneConfig;
    }
    void WidthValueChanged(ChangeEvent<int> evt){
        w=evt.newValue;
    }
    void HeightValueChanged(ChangeEvent<int> evt){
        h=evt.newValue;
    }
    void OnClick(){
        AddPrefabToConfig(prefab);
    }
    void AddPrefabsInDirectoryOnClick(){
        ProcessPrefabsInDirectory();
    }
}