using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class TestRoomPrefabGenerator : EditorWindow
{
    RoomSceneConfig config;
    GameObject prefab;
    float roomWidth, strokeWidth;
    RoomType[] roomTypes;
    [MenuItem("Test/Test Room Prefab Generator")]
    // Start is called before the first frame update
    static void Init()
    {
        TestRoomPrefabGenerator window = (TestRoomPrefabGenerator)EditorWindow.GetWindow(typeof(TestRoomPrefabGenerator));
        window.Show();
    }
    private void OnEnable()
    {
        roomTypes = new RoomType[]
        {
            RoomType.S1x1U1R1D1,
            RoomType.S1x1R1D1L1,
            RoomType.S1x1U1D1L1,
            RoomType.S1x1U1R1L1,
            RoomType.S1x1U1R1,
            RoomType.S1x1U1D1,
            RoomType.S1x1U1L1,
            RoomType.S1x1R1D1,
            RoomType.S1x1R1L1,
            RoomType.S1x1D1L1,
            RoomType.S1x1U1,
            RoomType.S1x1R1,
            RoomType.S1x1D1,
            RoomType.S1x1L1,

            RoomType.S1x2U1R1D1,
            RoomType.S1x2U1R2D1,
            RoomType.S1x2R1D1L1,
            RoomType.S1x2R1D1L2,
            RoomType.S1x2R2D1L1,
            RoomType.S1x2R2D1L2,
            RoomType.S1x2U1D1L1,
            RoomType.S1x2U1D1L2,
            RoomType.S1x2U1R1L1,
            RoomType.S1x2U1R1L2,
            RoomType.S1x2U1R2L1,
            RoomType.S1x2U1R2L2,
            RoomType.S1x2U1R1,
            RoomType.S1x2U1R2,
            RoomType.S1x2U1D1,
            RoomType.S1x2U1L1,
            RoomType.S1x2U1L2,
            RoomType.S1x2R1D1,
            RoomType.S1x2R2D1,
            RoomType.S1x2R1L1,
            RoomType.S1x2R1L2,
            RoomType.S1x2R2L1,
            RoomType.S1x2R2L2,
            RoomType.S1x2D1L1,
            RoomType.S1x2D1L2,
            RoomType.S1x2U1,
            RoomType.S1x2R1,
            RoomType.S1x2R2,
            RoomType.S1x2D1,
            RoomType.S1x2L1,
            RoomType.S1x2L2,

            RoomType.S2x1U1R1D1,
            RoomType.S2x1U1R1D2,
            RoomType.S2x1U2R1D1,
            RoomType.S2x1U2R1D2,
            RoomType.S2x1R1D1L1,
            RoomType.S2x1R1D2L1,
            RoomType.S2x1U1D1L1,
            RoomType.S2x1U1D2L1,
            RoomType.S2x1U2D1L1,
            RoomType.S2x1U2D2L1,
            RoomType.S2x1U1R1L1,
            RoomType.S2x1U2R1L1,
            RoomType.S2x1U1R1,
            RoomType.S2x1U2R1,
            RoomType.S2x1U1D1,
            RoomType.S2x1U1D2,
            RoomType.S2x1U2D1,
            RoomType.S2x1U2D2,
            RoomType.S2x1U1L1,
            RoomType.S2x1U2L1,
            RoomType.S2x1R1D1,
            RoomType.S2x1R1D2,
            RoomType.S2x1R1L1,
            RoomType.S2x1D1L1,
            RoomType.S2x1D2L1,
            RoomType.S2x1U1,
            RoomType.S2x1U2,
            RoomType.S2x1R1,
            RoomType.S2x1D1,
            RoomType.S2x1D2,
            RoomType.S2x1L1,

            RoomType.S2x2U1R1D1,
            RoomType.S2x2U1R1D2,
            RoomType.S2x2U1R2D1,
            RoomType.S2x2U1R2D2,
            RoomType.S2x2U2R1D1,
            RoomType.S2x2U2R1D2,
            RoomType.S2x2U2R2D1,
            RoomType.S2x2U2R2D2,
            RoomType.S2x2R1D1L1,
            RoomType.S2x2R1D1L2,
            RoomType.S2x2R1D2L1,
            RoomType.S2x2R1D2L2,
            RoomType.S2x2R2D1L1,
            RoomType.S2x2R2D1L2,
            RoomType.S2x2R2D2L1,
            RoomType.S2x2R2D2L2,
            RoomType.S2x2U1D1L1,
            RoomType.S2x2U1D1L2,
            RoomType.S2x2U1D2L1,
            RoomType.S2x2U1D2L2,
            RoomType.S2x2U2D1L1,
            RoomType.S2x2U2D1L2,
            RoomType.S2x2U2D2L1,
            RoomType.S2x2U2D2L2,
            RoomType.S2x2U1R1L1,
            RoomType.S2x2U1R1L2,
            RoomType.S2x2U1R2L1,
            RoomType.S2x2U1R2L2,
            RoomType.S2x2U2R1L1,
            RoomType.S2x2U2R1L2,
            RoomType.S2x2U2R2L1,
            RoomType.S2x2U2R2L2,
            RoomType.S2x2U1R1,
            RoomType.S2x2U1R2,
            RoomType.S2x2U2R1,
            RoomType.S2x2U2R2,
            RoomType.S2x2U1D1,
            RoomType.S2x2U1D2,
            RoomType.S2x2U2D1,
            RoomType.S2x2U2D2,
            RoomType.S2x2U1L1,
            RoomType.S2x2U1L2,
            RoomType.S2x2U2L1,
            RoomType.S2x2U2L2,
            RoomType.S2x2R1D1,
            RoomType.S2x2R1D2,
            RoomType.S2x2R2D1,
            RoomType.S2x2R2D2,
            RoomType.S2x2R1L1,
            RoomType.S2x2R1L2,
            RoomType.S2x2R2L1,
            RoomType.S2x2R2L2,
            RoomType.S2x2D1L1,
            RoomType.S2x2D1L2,
            RoomType.S2x2D2L1,
            RoomType.S2x2D2L2,
            RoomType.S2x2U1,
            RoomType.S2x2U2,
            RoomType.S2x2R1,
            RoomType.S2x2R2,
            RoomType.S2x2D1,
            RoomType.S2x2D2,
            RoomType.S2x2L1,
            RoomType.S2x2L2,

            RoomType.S3x3L1,
            RoomType.S3x3L2,
            RoomType.S3x3L3,
            RoomType.S3x3R1,
            RoomType.S3x3R2,
            RoomType.S3x3R3,

            RoomType.S4x4L1,
            RoomType.S4x4L2,
            RoomType.S4x4L3,
            RoomType.S4x4L4,
            RoomType.S4x4R1,
            RoomType.S4x4R2,
            RoomType.S4x4R3,
            RoomType.S4x4R4,
        };
    }
    GameObject GenerateWall(float left, float top, float right, float bottom, Transform parent, int doorNum, int dir)
    {
        GameObject ret = Instantiate(prefab, parent);
        ret.name = "wall";
        Vector3 scale = new Vector3(right - left, top - bottom, 1);
        ret.transform.localScale = scale;
        ret.transform.localPosition = new Vector3((right + left) / 2, (top + bottom) / 2, 0);
        if (doorNum > 0)
        {
            GameObject door = Instantiate(prefab, parent);
            door.name = "door";
            SpriteRenderer spr=door.GetComponent<SpriteRenderer>();
            spr.color = Color.red;
            spr.sortingOrder = 1;
            scale = new Vector3(strokeWidth, strokeWidth, 1);
            door.transform.localScale = scale;
            switch (dir)
            {
                case 0: door.transform.localPosition = new Vector3(roomWidth * doorNum - roomWidth / 2, ret.transform.position.y, 0); break;
                case 1: door.transform.localPosition = new Vector3(ret.transform.position.x, top - bottom - roomWidth * doorNum + roomWidth / 2, 0); break;
                case 2: door.transform.localPosition = new Vector3(right - left - roomWidth * doorNum + roomWidth / 2, ret.transform.position.y, 0); break;
                case 3: door.transform.localPosition = new Vector3(ret.transform.position.x, roomWidth * doorNum - roomWidth / 2, 0); break;
            }
        }
        return ret;
    }
    void GenerateRoomPrefabs()
    {
        for(int i = 0; i < roomTypes.Length; ++i)
        {
            RoomType type = roomTypes[i];
            int w = ((int)type & 0b111000000000000000) >> 15;
            int h = ((int)type & 0b111000000000000) >> 12;
            GameObject room = new GameObject($"Room {type.ToString()}");
            float l = 0, r = w * roomWidth, t = h * roomWidth, b = 0;
            GenerateWall(l, t, r, t - strokeWidth, room.transform, ((int)type>>9)&0b111, 0);
            GenerateWall(r-strokeWidth, t, r, b, room.transform, ((int)type>>6)&0b111, 1);
            GenerateWall(l, b+strokeWidth, r, b, room.transform, ((int)type>>3)&0b111, 2);
            GenerateWall(l, t, l+strokeWidth, b, room.transform, ((int)type)&0b111, 3);
            GameObject roomAsPrefab = PrefabUtility.SaveAsPrefabAsset(room, $"Assets/Prefabs/MapGeneration/{room.name}.prefab");
            DestroyImmediate(room);
            config.roomPrefabs_arrayList.Add(new RoomSceneConfig.Element(type, roomAsPrefab));
        }
    }
    private void OnGUI()
    {
        config = (RoomSceneConfig)EditorGUILayout.ObjectField("config", config, typeof(RoomSceneConfig), false);
        prefab = (GameObject)EditorGUILayout.ObjectField("2D square prefab", prefab, typeof(GameObject), true);
        roomWidth = EditorGUILayout.FloatField("roomWidth", roomWidth);
        strokeWidth = EditorGUILayout.FloatField("strokeWidth", strokeWidth);
        if (GUILayout.Button("generate room prefabs"))
            GenerateRoomPrefabs();
    }
}
