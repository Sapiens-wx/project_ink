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
    int w=24,h=13;
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

    Dictionary<Vector2Int,int> GetCells(Tilemap tilemap, out Vector2Int min, out Vector2Int max){
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
        Tilemap tilemap=go.transform.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap==null)
            tilemap=go.transform.Find("tilemap").GetComponent<Tilemap>();
        if(tilemap==null){
            Debug.LogError("cannot find the tilemap in "+go.name);
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
    void AddPrefabToConfig(GameObject prefab){
        if(config==null){
            Debug.LogError("you need to assign config");
            return;
        }
        if(prefab==null){
            Debug.LogError("you need to assign a room prefab");
            return;
        }
        RoomType type=GetType(prefab);
        if(type==RoomType.Error){
            Debug.LogError("room type is RoomType.Error. stopped");
            return;
        }
        int i=0;
        foreach(RoomSceneConfig.Element e in config.roomPrefabs_arrayList){
            //is the target type
            if(e.roomType==type){
                foreach(GameObject go in e.prefabs){
                    if(go==prefab){
                        Debug.LogError($"already contains {prefab.name} in index {i}.");
                        return;
                    }
                }
                e.prefabs.Add(prefab);
                Debug.Log($"{prefab.name} added in index {i} as type {type}");
                return;
            }
            ++i;
        }
        config.roomPrefabs_arrayList.Add(new RoomSceneConfig.Element(type, prefab));
        Debug.Log($"{prefab.name} added in index {i} as type {type}");
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

        UnityEngine.UIElements.Button button=new UnityEngine.UIElements.Button();
        button.text="test";
        button.clicked+=OnClick;
        root.Add(button);
    }
    void ValueChanged(ChangeEvent<Object> evt){
        prefab=evt.newValue as GameObject;
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
}