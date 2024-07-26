using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generate a tree of rooms. Does not generate things inside the rooms
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("Debug")]
    public int testTimes;
    private bool stop = false;
    [Header("Generate HUD Map")]
    public GameObject img_prefab;
    public Sprite spr_room1x1, spr_room1x2, spr_room2x1, spr_room2x2, spr_roomBoss;
    public int mapWidth, mapHeight;
    public Sprite spr_door;
    [Range(0f, 1f)]
    public float doorRatio; //ratio to the size of a grid. 0-1
    public Transform mapContainer, doorsParent;
    /// <summary>
    /// stores room prefabs with a dictionary<RoomType, List<GameObect>>
    /// </summary>
    [Header("Generate Room Scene")]
    public RoomSceneConfig roomSceneConfig;
    /// <summary>
    /// the width of a 1x1 room
    /// </summary>
    public float roomSceneWidth;
    /// <summary>
    /// parent of the room gameobects
    /// </summary>
    public Transform roomSceneParent;
    private void PrintGrid(Room[][] grid)
    {
        StringBuilder sb = new StringBuilder();
        int w = grid.Length, h = grid[0].Length;
        for (--h; h > -1; --h)
        {
            sb.Append(h.ToString());
            if (h > 9) sb.Append(" |");
            else sb.Append("  |");
            for(int i = 0; i < w; ++i)
            {
                if (grid[i][h] == null)
                    sb.Append('-');
                else sb.Append('#');
            }
            sb.Append('\n');
        }
        Debug.Log(sb);
    }
    [ContextMenu("test")]
    public void Test()
    {
        stop = false;
        for(int i = 0; i < testTimes && stop==false; ++i)
        {
            DeleteChildren();
            Room root = GenerateRoom();
            GenerateRoomMap(root, mapContainer);
            GenerateRoomScene(root);
        }
    }
    [ContextMenu("delete children")]
    public void DeleteChildren()
    {
        for(int i = roomSceneParent.childCount; i > 0; --i) 
            DestroyImmediate(roomSceneParent.GetChild(0).gameObject);
        for(int i = mapContainer.childCount; i > 0; --i) 
            DestroyImmediate(mapContainer.GetChild(0).gameObject);
        for(int i = doorsParent.childCount; i > 0; --i) 
            DestroyImmediate(doorsParent.GetChild(0).gameObject);
    }
    public void PrintRooms(Room root)
    {
        Queue<Room> q = new Queue<Room>();
        Room curRoom;
        q.Enqueue(root);
        int n = 1;
        int count = 0;
        while(n > 0)
        {
            Debug.Log($"------layer {count}-------");
            for(int i = 0; i < n; ++i)
            {
                curRoom = q.Dequeue();
                Debug.Log($"room {curRoom.w}x{curRoom.h} ({curRoom.x},{curRoom.y})");
                for(int j = 0; j < curRoom.children.Length; ++j)
                {
                    if (curRoom.children[j] != null)
                        q.Enqueue(curRoom.children[j]);
                }
            }
            n = q.Count;
            ++count;
        }
    }
    #region HUD map
    /// <summary>
    /// Generate a door HUD
    /// </summary>
    /// <param name="door"></param>
    /// <param name="parent">parent of the instantiated door HUD</param>
    /// <param name="doorWidth">size of a door (actual size)</param>
    /// <param name="roomWidth">size of a room (actual size, not 1x1 or 1x2)</param>
    /// <returns></returns>
    private GameObject GenerateDoorMap(Door door, Transform parent, float doorWidth, float roomWidth)
    {
        GameObject go = Instantiate(img_prefab, parent);
        go.name = $"door(dir: {door.dir}, from room ({door.fromRoom.x},{door.fromRoom.y}))";
        go.GetComponent<Image>().sprite = spr_door;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(doorWidth, doorWidth);
        //switch direction
        Vector2 doorOffset = Vector2.zero;
        float halfRoomWidth = roomWidth / 2;
        switch (door.dir)
        {
            case 0: doorOffset = new Vector2(0, -halfRoomWidth); break; //up
            case 1: doorOffset = new Vector2(-halfRoomWidth, 0); break; //right
            case 2: doorOffset = new Vector2(0, halfRoomWidth); break; //down
            case 3: doorOffset = new Vector2(halfRoomWidth, 0); break; //left
        }
        //+halfRoomWidth: the pivot of the rooms is at the left bottom of each recttransform. add (halfRoomWidth, halfRoomWidth) to make sure the door is generated on the sides of a room correctly
        rectTransform.position = new Vector2(door.x*roomWidth+doorOffset.x+halfRoomWidth, door.y*roomWidth+doorOffset.y+halfRoomWidth);
        rectTransform.SetParent(parent);
        return go;
    }
    /// <summary>
    /// generate room map HUD
    /// </summary>
    /// <param name="root"></param>
    /// <param name="parent"></param>
    public void GenerateRoomMap(Room root, Transform parent)
    {
        float wFactor = mapWidth / 23.0f; //grid width
        float hFactor = mapHeight / 45.0f; //grid height
        float doorWidth = wFactor * doorRatio;

        List<Transform> needsToBeCenteredObjects = new List<Transform>();
        Vector3 center = Vector3.zero; //for aligning the rooms. 
        int roomsCount = 0;

        Queue<Room> q = new Queue<Room>();
        Room curRoom;
        q.Enqueue(root);
        while (q.Count > 0)
        {
            curRoom = q.Dequeue();
            //generate map
            GameObject go = GameObject.Instantiate(img_prefab, mapContainer);
            go.name = $"{curRoom.w}x{curRoom.h} ({curRoom.x},{curRoom.y})";
            go.transform.SetParent(parent);
            Image img = go.GetComponent<Image>();
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            needsToBeCenteredObjects.Add(go.transform);
            RoomType roomSizeInfo=curRoom.GetRoomType()&RoomType.OnlySizeInfo;
            switch (roomSizeInfo)
            {
                case RoomType.S1x1: img.sprite = spr_room1x1; break;
                case RoomType.S1x2: img.sprite = spr_room1x2; break;
                case RoomType.S2x1: img.sprite = spr_room2x1; break;
                case RoomType.S2x2: img.sprite = spr_room2x2; break;
                case RoomType.S4x4: img.sprite = spr_roomBoss; break;
                default: throw new Exception("RoomGenerator.GenerateRoomMap(): wrong room size info"); break;
            }
            go.transform.position = new Vector3(curRoom.x * wFactor, curRoom.y * hFactor);
            center += go.transform.position; //add this room's position to center (Vector3), then calculate actual center after the loop
            rectTransform.sizeDelta = new Vector2(curRoom.w*wFactor, curRoom.h*hFactor);
            for(int i = 0; i < curRoom.children.Length; ++i)
            {
                if (curRoom.children[i] != null)
                    q.Enqueue(curRoom.children[i]);
            }
            if (curRoom.doors != null)
            {
                for (int i = 0; i < curRoom.doors.Length; ++i)
                {
                    if (curRoom.doors[i] != null && curRoom.doors[i].toRoom != null)
                        needsToBeCenteredObjects.Add(GenerateDoorMap(curRoom.doors[i], doorsParent, doorWidth, wFactor).transform);
                }
            }
            ++roomsCount;
        }
        center = center / roomsCount - parent.position;
        for(int i = 0; i<needsToBeCenteredObjects.Count; ++i)
        {
            needsToBeCenteredObjects[i].position -= center; //make the rooms images center around their parent
        }
    }
    #endregion
    #region Room Scene
    private List<GameObject> GetRoomPrefabs(Room room)
    {
        RoomType type = room.GetRoomType();
        return roomSceneConfig.roomPrefabs_dictionary[type];
    }
    public void GenerateRoomScene(Room root)
    {
        roomSceneConfig.InitializeDictionary();
        List<Transform> needsToBeCenteredObjects = new List<Transform>();
        Vector3 center = Vector3.zero; //for aligning the rooms. 
        int roomsCount = 0;

        Queue<Room> q = new Queue<Room>();
        Room curRoom;
        q.Enqueue(root);
        while (q.Count > 0)
        {
            curRoom = q.Dequeue();
            //generate map
            List<GameObject> roomPrefabs = GetRoomPrefabs(curRoom);
            GameObject roomScene = Instantiate(roomPrefabs[UnityEngine.Random.Range(0, roomPrefabs.Count)], roomSceneParent); //randomly selected one
            roomScene.name = $"{curRoom.w}x{curRoom.h} ({curRoom.x},{curRoom.y})";
            roomScene.transform.position = new Vector3(curRoom.x * roomSceneWidth, curRoom.y * roomSceneWidth, 0);
            needsToBeCenteredObjects.Add(roomScene.transform);
            center += roomScene.transform.position; //add this room's position to center (Vector3), then calculate actual center after the loop
            for(int i = 0; i < curRoom.children.Length; ++i)
            {
                if (curRoom.children[i] != null)
                    q.Enqueue(curRoom.children[i]);
            }
            //generate doors.
            /*
            if (curRoom.doors != null)
            {
                for (int i = 0; i < curRoom.doors.Length; ++i)
                {
                    if (curRoom.doors[i] != null && curRoom.doors[i].toRoom != null)
                        needsToBeCenteredObjects.Add(GenerateDoorMap(curRoom.doors[i], doorsParent, doorWidth, wFactor).transform);
                }
            }
            */
            ++roomsCount;
        }
        center = center / roomsCount - roomSceneParent.position;
        for(int i = 0; i<needsToBeCenteredObjects.Count; ++i)
        {
            needsToBeCenteredObjects[i].position -= center; //make the rooms images center around their parent
        }
    }
    #endregion
    #region Room tree
    /// <summary>
    /// Generates a boss room after all normal rooms are generated
    /// </summary>
    private void GenerateBossRoom(Room[][] roomGrid, Queue<Door> doorCandidates)
    {
        Room bossRoom = new Room(4, 4);
        int debug_____queueCount = doorCandidates.Count;
        while (true)
        {
            if (doorCandidates.Count == 0)
            {
                //
                Debug.LogError("doorcandidates.count==0, originalCount=" + debug_____queueCount.ToString()); ;
                stop = true;
                return;
            }
            Door curDoor = doorCandidates.Dequeue();
            if(UnityEngine.Random.Range(0,3)>0)
            {
                doorCandidates.Enqueue(curDoor);
                continue;
            }
            List<Vector3Int> positions = curDoor.GetPossibleBossRooms(roomGrid, bossRoom);
            if (positions == null || positions.Count == 0)
                continue;
            Vector3Int selectedPosition = positions[UnityEngine.Random.Range(0, positions.Count)];
            bossRoom.x = selectedPosition.x;
            bossRoom.y = selectedPosition.y;
            curDoor.toRoom = bossRoom;
            curDoor.fromRoom.children[curDoor.fromRoomChildIndex] = bossRoom;
            curDoor.toRoom.SetParent(curDoor.fromRoom, curDoor);
            RegisterRoomOnGrid(roomGrid, bossRoom);
            break;
        }
    }
    /// <summary>
    /// generate a tree of Rooms.
    /// </summary>
    /// <returns>root of the tree</returns>
    public Room GenerateRoom()
    {
        //initialize grid
        Room[][] roomGrid = new Room[23][];
        for (int i = 0; i < 23; ++i) {
            roomGrid[i] = new Room[45];
        }
        //room templates and counts
        int totalCount = 14;
        int[] roomCounts = new int[4];
        roomCounts[0] = 6;
        roomCounts[1] = 3;
        roomCounts[2] = 3;
        roomCounts[3] = 2;
        Room[] roomTemplates = new Room[4];
        roomTemplates[0] = new Room(1, 1);
        roomTemplates[1] = new Room(1, 2);
        roomTemplates[2] = new Room(2, 1);
        roomTemplates[3] = new Room(2, 2);

        Room root = new Room(1, 1, 0, 23);
        Queue<Door> q=new Queue<Door>();
        RandomlyEnqueue(q, root.GenerateDoorsRandomly(3));
        RegisterRoomOnGrid(roomGrid, root);

        List<Vector3Int> roomList_position = new List<Vector3Int>();
        while (totalCount > 0)
        {
            roomList_position.Clear();
            Door curDoor = q.Dequeue();
            List<Room> roomList = curDoor.GetPossibleRooms(roomGrid, roomCounts, roomTemplates, roomList_position);
            if (roomList == null || roomList.Count==0) continue;
            if(UnityEngine.Random.Range(0, 3) > 0)
            {
                q.Enqueue(curDoor);
                continue;
            }
            int randomN = UnityEngine.Random.Range(0, roomList.Count);
            Room selectedRoom = new Room(roomList[randomN]);
            Vector3Int position = roomList_position[randomN];
            selectedRoom.x = position.x;
            selectedRoom.y = position.y;
            RegisterRoomOnGrid(roomGrid, selectedRoom);
            curDoor.toRoom = selectedRoom;
            curDoor.fromRoom.children[curDoor.fromRoomChildIndex] = selectedRoom;
            curDoor.toRoom.SetParent(curDoor.fromRoom, curDoor);
            --roomCounts[(selectedRoom.w << 1) + selectedRoom.h - 3];
            --totalCount;
            RandomlyEnqueue(q, selectedRoom.GenerateDoorsRandomly(position.z));
        }
        GenerateBossRoom(roomGrid, q);
        return root;
    }
    private void RegisterRoomOnGrid(Room[][] grid, Room room)
    {
        int l = room.x, r = l + room.w;
        int b = room.y, t = b + room.h;
        for (; l < r; ++l)
        {
            for (int j=b; j < t; ++j)
            {
                grid[l][j] = room;
            }
        }
    }
    private static void RandomlyEnqueue(Queue<Door> q, params Door[] doors)
    {
        float[] keys = new float[doors.Length];
        for(int i = 0; i < doors.Length; ++i)
        {
            keys[i] = UnityEngine.Random.Range(0.0f, 1.0f);
        }
        Array.Sort(keys, doors);
        for (int i = 0; i < keys.Length; ++i)
        {
            q.Enqueue(doors[i]);
        }
    }
    #endregion
}

public class Door
{
    public Room fromRoom, toRoom;
    public int fromRoomChildIndex;
    /// <summary>
    /// direction of the door
    /// </summary>
    public int dir;
    public int x, y;
    public Door(Room from, int direction, int fromRoomChildIndex)
    {
        fromRoom = from;
        dir = direction;
        this.fromRoomChildIndex = fromRoomChildIndex;
    }
    public Door(Room from, int direction, int _x, int _y, int fromRoomChildIndex)
    {
        fromRoom = from;
        dir = direction;
        x = _x;
        y = _y;
        this.fromRoomChildIndex = fromRoomChildIndex;
    }
    public void SetPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    /// <param name="grid"></param>
    /// <param name="counts"></param>
    /// <param name="roomTemplates"></param>
    /// <param name="positions">positions.z is the direction of parent room relative to the generated room</param>
    /// <returns></returns>
    public List<Room> GetPossibleRooms(Room[][] grid, int[] counts, Room[] roomTemplates, List<Vector3Int> positions)
    {
        if (x < 0 || y < 0 || x >= grid.Length || y >= grid[0].Length) return null;
        if (grid[x][y] != null) return null;
        List<Room> ret = new List<Room>();
        Vector3Int position;
        for(int i = 0; i < counts.Length; ++i)
        {
            if (counts[i]>0)
            {
                if (roomTemplates[i].CheckOverlaps(this, grid, 0, out position))
                {
                    positions.Add(position);
                    ret.Add(roomTemplates[i]);
                }
                if (roomTemplates[i].CheckOverlaps(this, grid, 1, out position))
                {
                    positions.Add(position);
                    ret.Add(roomTemplates[i]);
                }
            }
        }
        return ret;
    }
    /// <summary>
    /// Called by RoomGenerator::GenerateBossRoom. different from GetPossibleRooms because boss room can only be connected horizontally.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="bossRoom"></param>
    /// <returns></returns>
    public List<Vector3Int> GetPossibleBossRooms(Room[][] grid, Room bossRoom)
    {
        if (dir == 0 || dir == 2) return null;
        if (x < 0 || y < 0 || x >= grid.Length || y >= grid[0].Length) return null;
        if (grid[x][y] != null) return null;
        List<Vector3Int> ret = new List<Vector3Int>();
        Vector3Int position;
        if (bossRoom.CheckOverlaps(this, grid, 0, out position))
            ret.Add(position);
        if (bossRoom.CheckOverlaps(this, grid, 1, out position))
            ret.Add(position);
        return ret;
    }
}
// how is the size and door position matched to a binary number?
// width[000]height[000]up[000]right[000]down[000]left[000]
// exampmle: 1x2 with up, right, down doors: 001 010 001 010 001 000
// --  --
// |    |
// |    |
// |    |
// |     
// --  --
/// <summary>
/// bit (from left to right): width[000] height [000] up[000] right[000] down[000] left[000].
/// </summary>
public enum RoomType
{
    S1x1U1R1D1=0b1001001001001000,
    S1x1R1D1L1=0b1001000001001001,
    S1x1U1D1L1=0b1001001000001001,
    S1x1U1R1L1=0b1001001001000001,
    S1x1U1R1=0b1001001001000000,
    S1x1U1D1=0b1001001000001000,
    S1x1U1L1=0b1001001000000001,
    S1x1R1D1=0b1001000001001000,
    S1x1R1L1=0b1001000001000001,
    S1x1D1L1=0b1001000000001001,
    S1x1U1=0b1001001000000000,
    S1x1R1=0b1001000001000000,
    S1x1D1=0b1001000000001000,
    S1x1L1=0b1001000000000001,

    S1x2U1R1D1=0b1010001001001000,
    S1x2U1R2D1=0b1010001010001000,
    S1x2R1D1L1=0b1010000001001001,
    S1x2R1D1L2=0b1010000001001010,
    S1x2R2D1L1=0b1010000010001001,
    S1x2R2D1L2=0b1010000010001010,
    S1x2U1D1L1=0b1010001000001001,
    S1x2U1D1L2=0b1010001000001010,
    S1x2U1R1L1=0b1010001001000001,
    S1x2U1R1L2=0b1010001001000010,
    S1x2U1R2L1=0b1010001010000001,
    S1x2U1R2L2=0b1010001010000010,
    S1x2U1R1=0b1010001001000000,
    S1x2U1R2=0b1010001010000000,
    S1x2U1D1=0b1010001000001000,
    S1x2U1L1=0b1010001000000001,
    S1x2U1L2=0b1010001000000010,
    S1x2R1D1=0b1010000001001000,
    S1x2R2D1=0b1010000010001000,
    S1x2R1L1=0b1010000001000001,
    S1x2R1L2=0b1010000001000010,
    S1x2R2L1=0b1010000010000001,
    S1x2R2L2=0b1010000010000010,
    S1x2D1L1=0b1010000000001001,
    S1x2D1L2=0b1010000000001010,
    S1x2U1=0b1010001000000000,
    S1x2R1=0b1010000001000000,
    S1x2R2=0b1010000010000000,
    S1x2D1=0b1010000000001000,
    S1x2L1=0b1010000000000001,
    S1x2L2=0b1010000000000010,

    S2x1U1R1D1=0b10001001001001000,
    S2x1U1R1D2=0b10001001001010000,
    S2x1U2R1D1=0b10001010001001000,
    S2x1U2R1D2=0b10001010001010000,
    S2x1R1D1L1=0b10001000001001001,
    S2x1R1D2L1=0b10001000001010001,
    S2x1U1D1L1=0b10001001000001001,
    S2x1U1D2L1=0b10001001000010001,
    S2x1U2D1L1=0b10001010000001001,
    S2x1U2D2L1=0b10001010000010001,
    S2x1U1R1L1=0b10001001001000001,
    S2x1U2R1L1=0b10001010001000001,
    S2x1U1R1=0b10001001001000000,
    S2x1U2R1=0b10001010001000000,
    S2x1U1D1=0b10001001000001000,
    S2x1U1D2=0b10001001000010000,
    S2x1U2D1=0b10001010000001000,
    S2x1U2D2=0b10001010000010000,
    S2x1U1L1=0b10001001000000001,
    S2x1U2L1=0b10001010000000001,
    S2x1R1D1=0b10001000001001000,
    S2x1R1D2=0b10001000001010000,
    S2x1R1L1=0b10001000001000001,
    S2x1D1L1=0b10001000000001001,
    S2x1D2L1=0b10001000000010001,
    S2x1U1=0b10001001000000000,
    S2x1U2=0b10001010000000000,
    S2x1R1=0b10001000001000000,
    S2x1D1=0b10001000000001000,
    S2x1D2=0b10001000000010000,
    S2x1L1=0b10001000000000001,

    S2x2U1R1D1=0b10010001001001000,
    S2x2U1R1D2=0b10010001001010000,
    S2x2U1R2D1=0b10010001010001000,
    S2x2U1R2D2=0b10010001010010000,
    S2x2U2R1D1=0b10010010001001000,
    S2x2U2R1D2=0b10010010001010000,
    S2x2U2R2D1=0b10010010010001000,
    S2x2U2R2D2=0b10010010010010000,
    S2x2R1D1L1=0b10010000001001001,
    S2x2R1D1L2=0b10010000001001010,
    S2x2R1D2L1=0b10010000001010001,
    S2x2R1D2L2=0b10010000001010010,
    S2x2R2D1L1=0b10010000010001001,
    S2x2R2D1L2=0b10010000010001010,
    S2x2R2D2L1=0b10010000010010001,
    S2x2R2D2L2=0b10010000010010010,
    S2x2U1D1L1=0b10010001000001001,
    S2x2U1D1L2=0b10010001000001010,
    S2x2U1D2L1=0b10010001000010001,
    S2x2U1D2L2=0b10010001000010010,
    S2x2U2D1L1=0b10010010000001001,
    S2x2U2D1L2=0b10010010000001010,
    S2x2U2D2L1=0b10010010000010001,
    S2x2U2D2L2=0b10010010000010010,
    S2x2U1R1L1=0b10010001001000001,
    S2x2U1R1L2=0b10010001001000010,
    S2x2U1R2L1=0b10010001010000001,
    S2x2U1R2L2=0b10010001010000010,
    S2x2U2R1L1=0b10010010001000001,
    S2x2U2R1L2=0b10010010001000010,
    S2x2U2R2L1=0b10010010010000001,
    S2x2U2R2L2=0b10010010010000010,
    S2x2U1R1=0b10010001001000000,
    S2x2U1R2=0b10010001010000000,
    S2x2U2R1=0b10010010001000000,
    S2x2U2R2=0b10010010010000000,
    S2x2U1D1=0b10010001000001000,
    S2x2U1D2=0b10010001000010000,
    S2x2U2D1=0b10010010000001000,
    S2x2U2D2=0b10010010000010000,
    S2x2U1L1=0b10010001000000001,
    S2x2U1L2=0b10010001000000010,
    S2x2U2L1=0b10010010000000001,
    S2x2U2L2=0b10010010000000010,
    S2x2R1D1=0b10010000001001000,
    S2x2R1D2=0b10010000001010000,
    S2x2R2D1=0b10010000010001000,
    S2x2R2D2=0b10010000010010000,
    S2x2R1L1=0b10010000001000001,
    S2x2R1L2=0b10010000001000010,
    S2x2R2L1=0b10010000010000001,
    S2x2R2L2=0b10010000010000010,
    S2x2D1L1=0b10010000000001001,
    S2x2D1L2=0b10010000000001010,
    S2x2D2L1=0b10010000000010001,
    S2x2D2L2=0b10010000000010010,
    S2x2U1=0b10010001000000000,
    S2x2U2=0b10010010000000000,
    S2x2R1=0b10010000001000000,
    S2x2R2=0b10010000010000000,
    S2x2D1=0b10010000000001000,
    S2x2D2=0b10010000000010000,
    S2x2L1=0b10010000000000001,
    S2x2L2=0b10010000000000010,

    S3x3L1=0b11011000000000001,
    S3x3L2=0b11011000000000010,
    S3x3L3=0b11011000000000011,
    S3x3R1=0b11011000001000000,
    S3x3R2=0b11011000010000000,
    S3x3R3=0b11011000011000000,

    S4x4L1=0b100100000000000001,
    S4x4L2=0b100100000000000010,
    S4x4L3=0b100100000000000011,
    S4x4L4=0b100100000000000100,
    S4x4R1=0b100100000001000000,
    S4x4R2=0b100100000010000000,
    S4x4R3=0b100100000011000000,
    S4x4R4=0b100100000100000000,

    //debug purposes, without doors, don't use when configuring rooms
    S1x1=0b001001000000000000,
    S1x2=0b001010000000000000,
    S2x1=0b010001000000000000,
    S2x2=0b010010000000000000,
    S3x3=0b011011000000000000,
    S4x4=0b100100000000000000,

    OnlySizeInfo=0b111111000000000000,
    OnlyDoorsInfo=0b111111111111,
}
public class Room
{
    public int w, h;
    /// <summary>
    /// 0: up, 1: right, 2: down, 3: left
    /// </summary>
    public Room[] children;
    public Door[] doors;
    private Room parent;
    /// <summary>
    /// parent's would-be index in Room[] children (parent is actually not in Room[] children). useful for getting the room's type (Room.GetRoomType).
    /// </summary>
    private int parentIndex;
    public int x, y;
    public Room Parent { get; }
    public int ParentIndex { get; }
    public Room()
    {
    }
    public Room(int width, int height)
    {
        w = width; h = height;
        children = new Room[(width+height)<<1];
    }
    public Room(int width, int height, int _x, int _y)
    {
        w = width; h = height;
        children = new Room[(width+height)<<1];
        x = _x;
        y = _y;
    }
    public Room(Room room)
    {
        w = room.w;
        h = room.h;
        children = new Room[room.children.Length];
        x = room.x;
        y = room.y;
    }
    /// <summary>
    /// set the parent room of this room
    /// </summary>
    /// <param name="door"></param>
    public void SetParent(Room _parent, Door door)
    {
        parent = _parent;
        switch (door.dir ^ 0b10)
        {
            case 0: parentIndex = door.x - x; break;
            case 1: parentIndex = w + h - door.y + y - 1; break;
            case 2: parentIndex = w + h + w - door.x + x - 1; break;
            case 3: parentIndex = w + h + w + door.y - y; break;
            default: throw new Exception("Room.SetParent: wrong direction");
        }
        if (parentIndex >= children.Length) throw new Exception($"SetParent: parentIndex outside bound: this={this}, door=({door.x},{door.y}) dir={door.dir}"); ;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="door"></param>
    /// <param name="grid"></param>
    /// <param name="align">0: left/top align, 1: right/bottom align</param>
    /// <returns></returns>
    public bool CheckOverlaps(Door door, Room[][] grid, int align, out Vector3Int position)
    {
        int grid_width = grid.Length, grid_height = grid[0].Length;
        int l, t, r, b;
        position = Vector3Int.zero;
        if(align == 0) {
            switch (door.dir)
            {
                case 0: //up
                    l = door.x; r = l + w;
                    b = door.y - 1; t = b + h;
                    break;
                case 1: //right
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    break;
                case 2: //down
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    break;
                case 3: //left
                    r = door.x + 1; l = r - w;
                    t = door.y; b = t - h;
                    break;
                default: return false;
            }
        }
        else {
            switch (door.dir)
            {
                case 0: //up
                    r = door.x + 1; l = r - w;
                    b = door.y - 1; t = b + h;
                    break;
                case 1: //right
                    l = door.x; r = l + w;
                    b = door.y - 1; t = b + h;
                    break;
                case 2: //down
                    r = door.x + 1; l = r - w;
                    t = door.y; b = t - h;
                    break;
                case 3: //left
                    r = door.x + 1; l = r - w;
                    b = door.y - 1; t = b + h;
                    break;
                default: return false;
            }
        }
        //dir^0b10 means to get the opposition direction: up[0b00] ^ 0b10 -> down[0b10], right[0b01] ^ 0b10 -> left[0b11]
        position.z = door.dir ^ 0b10; 
        for(int i = l; i < r; ++i)
        {
            if (i < 0 || i >= grid_width) return false;
            for(int j = t; j > b; --j)
            {
                if(j<0 || j >= grid_height) return false;
                if (grid[i][j] != null) return false;
            }
        }
        position.x = l;
        position.y = b+1;
        //Debug.Log($"CheckOverlaps(door({door.x},{door.y}), room{w}x{h}({position.x},{position.y}), align={align})");
        return true;
    }
    private static Door[] DeleteFromArray(Door[] arr, int index)
    {
        Door[] ret=new Door[arr.Length-1];
        int i = 0, j = 0;
        for (; i < arr.Length; ++i)
        {
            if (i != index) ret[j++] = arr[i];
        }
        return ret;
    }
    public Door[] GenerateDoorsRandomly(int except)
    {
        Door[] doors = new Door[4];
        int randomNum = UnityEngine.Random.Range(0, w);
        doors[0] = new Door(this, 0, x+randomNum, y + h, randomNum); //x+randomNum: pick a random position to place the door on the up side of the room
        randomNum = UnityEngine.Random.Range(0, h);
        doors[1] = new Door(this, 1, x + w, y + randomNum, w + h - randomNum - 1);
        randomNum = UnityEngine.Random.Range(0, w);
        doors[2] = new Door(this, 2, x + randomNum, y - 1, (w << 1) + h - 1 - randomNum);
        randomNum = UnityEngine.Random.Range(0, h);
        doors[3] = new Door(this, 3, x - 1, y + randomNum, (w << 1) + h + randomNum);
        //先remove [except], 再随机remove一个门，因为一个房间最多三个门
        this.doors = DeleteFromArray(DeleteFromArray(doors, except), UnityEngine.Random.Range(0, 3));
        return this.doors;
    }
    public RoomType GetRoomType()
    {
        if (parent != null)
            children[parentIndex] = parent; //temporarily add parent into the children array, to count the room's side connecting to its parent as a door
        int ret = (w << 15) | (h << 12);
        int childrenIdx = 0;
        for(int i = 0; i < w; ++i) //up
        {
            if (children[childrenIdx] != null) {
                ret |= (i + 1) << 9;
                break;
            }
            ++childrenIdx;
        }
        childrenIdx = w;
        for(int i = 0; i < h; ++i) //right
        {
            if (children[childrenIdx] != null) {
                ret |= (i + 1) << 6;
                break;
            }
            ++childrenIdx;
        }
        childrenIdx = w + h;
        for(int i = 0; i < w; ++i) //down
        {
            if (children[childrenIdx] != null) {
                ret |= (i + 1) << 3;
                break;
            }
            ++childrenIdx;
        }
        childrenIdx = (w<<1)+h;
        for(int i = 0; i < h; ++i) //left
        {
            if (children[childrenIdx] != null) {
                ret |= i + 1;
                break;
            }
            ++childrenIdx;
        }
        if (parent != null)
            children[parentIndex] = null; //remove parent from the children array after counting.
        return (RoomType)ret;
    }
    public override string ToString()
    {
        return $"{w}x{h}({x},{y})";
    }
}
