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
        Room root = GenerateRoom();
        GenerateRoomMap(root, mapContainer);
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
    #region Room tree
    /// <summary>
    /// Generates a boss room after all normal rooms are generated
    /// </summary>
    private void GenerateBossRoom(Room[][] roomGrid, Queue<Door> doorCandidates)
    {
        Room bossRoom = new Room(4, 4);
        while (true)
        {
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
            curDoor.toRoom = bossRoom;
            curDoor.fromRoom.children[curDoor.fromRoomChildIndex] = bossRoom;
            bossRoom.x = selectedPosition.x;
            bossRoom.y = selectedPosition.y;
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
    public static Door[] GetDoorsFromRoom(Room room, int except)
    {
        Door[] ret;
        if (room.w == 1) {
            if (room.h == 1) {
                ret = new Door[4];
                for (int i = 0; i < 4; ++i)
                    ret[i] = new Door(room, i, i);
                ret[0].SetPosition(room.x, room.y + 1);
                ret[1].SetPosition(room.x + 1, room.y);
                ret[2].SetPosition(room.x, room.y - 1);
                ret[3].SetPosition(room.x - 1, room.y);
            }
            else {
                ret = new Door[4];
                ret[0] = new Door(room, 0, room.x, room.y+2,0);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[1] = new Door(room, 1, room.x+1, room.y+1,1);
                else
                    ret[1] = new Door(room, 1, room.x+1, room.y,2);
                ret[2] = new Door(room, 2, room.x, room.y-1,3);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[3] = new Door(room, 3, room.x-1, room.y+1,5);
                else
                    ret[3] = new Door(room, 3, room.x-1, room.y,4);
            }
        }
        else {
            if (room.h == 2) {
                ret = new Door[4];
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[0] = new Door(room, 0, room.x, room.y+2, 0);
                else
                    ret[0] = new Door(room, 0, room.x+1, room.y+2, 1);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[1] = new Door(room, 1, room.x + 2, room.y+1, 2);
                else
                    ret[1] = new Door(room, 1, room.x + 2, room.y, 3);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[2] = new Door(room, 2, room.x + 1, room.y-1, 4);
                else
                    ret[2] = new Door(room, 2, room.x, room.y-1, 5);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    ret[3] = new Door(room, 3, room.x-1, room.y, 6);
                else
                    ret[3] = new Door(room, 3, room.x-1, room.y+1, 7);
            }
            else {
                ret = new Door[4];
                if(UnityEngine.Random.Range(0,2)==0)
                    ret[0] = new Door(room, 0, room.x, room.y+1,0);
                else
                    ret[0] = new Door(room, 0, room.x+1, room.y+1,1);
                ret[1] = new Door(room, 1, room.x+2, room.y,2);
                if(UnityEngine.Random.Range(0,2)==0)
                    ret[2] = new Door(room, 2, room.x+1, room.y-1,3);
                else
                    ret[2] = new Door(room, 2, room.x, room.y-1,4);
                ret[3] = new Door(room, 3, room.x-1, room.y,5);
            }
        }
        //先remove [except], 再随机remove一个门，因为一个房间最多三个门
        return DeleteFromArray(DeleteFromArray(ret, except), UnityEngine.Random.Range(0, 3));
    }
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
// width[000]height[000]up[0]right[0]down[0]left[0]
// exampmle: 1x2 width up, right, down doors: 001 010 1 1 1 0
/// <summary>
/// first four digits: size of the room.
/// last four digits: does the room have door on its up, right, down, left side respectively.
/// bit digit (from left to right): width[000] height [000] up[0] right[0] down[0] left[0].
/// </summary>
public enum RoomType
{
    S1x1URD=0b0010011110,
    S1x1RDL=0b0010010111,
    S1x1UDL=0b0010011011,
    S1x1URL=0b0010011101,
    S1x1UR=0b0010011100,
    S1x1UD=0b0010011010,
    S1x1UL=0b0010011001,
    S1x1RD=0b0010010110,
    S1x1RL=0b0010010101,
    S1x1DL=0b0010010011,
    S1x1U=0b0010011000,
    S1x1R=0b0010010100,
    S1x1D=0b0010010010,
    S1x1L=0b0010010001,

    S1x2URD=0b0010101110,
    S1x2RDL=0b0010100111,
    S1x2UDL=0b0010101011,
    S1x2URL=0b0010101101,
    S1x2UR=0b0010101100,
    S1x2UD=0b0010101010,
    S1x2UL=0b0010101001,
    S1x2RD=0b0010100110,
    S1x2RL=0b0010100101,
    S1x2DL=0b0010100011,
    S1x2U=0b0010101000,
    S1x2R=0b0010100100,
    S1x2D=0b0010100010,
    S1x2L=0b0010100001,

    S2x1URD=0b0100011110,
    S2x1RDL=0b0100010111,
    S2x1UDL=0b0100011011,
    S2x1URL=0b0100011101,
    S2x1UR=0b0100011100,
    S2x1UD=0b0100011010,
    S2x1UL=0b0100011001,
    S2x1RD=0b0100010110,
    S2x1RL=0b0100010101,
    S2x1DL=0b0100010011,
    S2x1U=0b0100011000,
    S2x1R=0b0100010100,
    S2x1D=0b0100010010,
    S2x1L=0b0100010001,

    S2x2URD=0b0100101110,
    S2x2RDL=0b0100100111,
    S2x2UDL=0b0100101011,
    S2x2URL=0b0100101101,
    S2x2UR=0b0100101100,
    S2x2UD=0b0100101010,
    S2x2UL=0b0100101001,
    S2x2RD=0b0100100110,
    S2x2RL=0b0100100101,
    S2x2DL=0b0100100011,
    S2x2U=0b0100101000,
    S2x2R=0b0100100100,
    S2x2D=0b0100100010,
    S2x2L=0b0100100001,

    S3x3L=0b0110110001,
    S3x3R=0b0110110100,
    S4x4L=0b1001000001,
    S4x4R=0b1001000100,
    //debug purposes, without doors, don't use when configuring rooms
    S1x1=0b0010010000,
    S1x2=0b0010100000,
    S2x1=0b0100010000,
    S2x2=0b0100100000,
    S3x3=0b0110110000,
    S4x4=0b1001000000,

    SURD=0b1110,
    SRDL=0b0111,
    SUDL=0b1011,
    SURL=0b1101,
    SUR=0b1100,
    SUD=0b1010,
    SUL=0b1001,
    SRD=0b0110,
    SRL=0b0101,
    SDL=0b0011,
    SU=0b1000,
    SR=0b0100,
    SD=0b0010,
    SL=0b0001,

    OnlySizeInfo=0b1111110000,
    OnlyDoorsInfo=0b1111
}
public class Room
{
    public int w, h;
    /// <summary>
    /// 0: up, 1: right, 2: down, 3: left
    /// </summary>
    public Room[] children;
    public Door[] doors;
    public int x, y;
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
        if (w == 1) {
            if (h == 1) {
                for (int i = 0; i < 4; ++i)
                    doors[i] = new Door(this, i, i);
                doors[0].SetPosition(x, y + 1);
                doors[1].SetPosition(x + 1, y);
                doors[2].SetPosition(x, y - 1);
                doors[3].SetPosition(x - 1, y);
            }
            else {
                doors[0] = new Door(this, 0, x, y+2,0);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[1] = new Door(this, 1, x+1, y+1,1);
                else
                    doors[1] = new Door(this, 1, x+1, y,2);
                doors[2] = new Door(this, 2, x, y-1,3);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[3] = new Door(this, 3, x-1, y+1,5);
                else
                    doors[3] = new Door(this, 3, x-1, y,4);
            }
        }
        else {
            if (h == 2) {
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[0] = new Door(this, 0, x, y+2, 0);
                else
                    doors[0] = new Door(this, 0, x+1, y+2, 1);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[1] = new Door(this, 1, x + 2, y+1, 2);
                else
                    doors[1] = new Door(this, 1, x + 2, y, 3);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[2] = new Door(this, 2, x + 1, y-1, 4);
                else
                    doors[2] = new Door(this, 2, x, y-1, 5);
                if (UnityEngine.Random.Range(0, 2) == 0)
                    doors[3] = new Door(this, 3, x-1, y, 6);
                else
                    doors[3] = new Door(this, 3, x-1, y+1, 7);
            }
            else {
                if(UnityEngine.Random.Range(0,2)==0)
                    doors[0] = new Door(this, 0, x, y+1,0);
                else
                    doors[0] = new Door(this, 0, x+1, y+1,1);
                doors[1] = new Door(this, 1, x+2, y,2);
                if(UnityEngine.Random.Range(0,2)==0)
                    doors[2] = new Door(this, 2, x+1, y-1,3);
                else
                    doors[2] = new Door(this, 2, x, y-1,4);
                doors[3] = new Door(this, 3, x-1, y,5);
            }
        }
        //先remove [except], 再随机remove一个门，因为一个房间最多三个门
        this.doors = DeleteFromArray(DeleteFromArray(doors, except), UnityEngine.Random.Range(0, 3));
        return this.doors;
    }
    public RoomType GetRoomType()
    {
        int ret = (w << 7) | (h << 4);
        switch ((RoomType)ret)
        {
            case RoomType.S1x1:
                ret |= (children[0] == null) ? 0 : (int)RoomType.S1x1U;
                ret |= (children[1] == null) ? 0 : (int)RoomType.S1x1R;
                ret |= (children[2] == null) ? 0 : (int)RoomType.S1x1D;
                ret |= (children[3] == null) ? 0 : (int)RoomType.S1x1L;
                break;
            case RoomType.S1x2:
                ret |= (children[0] == null) ? 0 : (int)RoomType.S1x2U;
                ret |= (children[1] == null) ? 0 : (int)RoomType.S1x2R;
                ret |= (children[2] == null) ? 0 : (int)RoomType.S1x2R;
                ret |= (children[3] == null) ? 0 : (int)RoomType.S1x2D;
                ret |= (children[4] == null) ? 0 : (int)RoomType.S1x2L;
                ret |= (children[5] == null) ? 0 : (int)RoomType.S1x2L;
                break;
            case RoomType.S2x1:
                ret |= (children[0] == null) ? 0 : (int)RoomType.S2x1U;
                ret |= (children[1] == null) ? 0 : (int)RoomType.S2x1U;
                ret |= (children[2] == null) ? 0 : (int)RoomType.S2x1R;
                ret |= (children[3] == null) ? 0 : (int)RoomType.S2x1D;
                ret |= (children[4] == null) ? 0 : (int)RoomType.S2x1D;
                ret |= (children[5] == null) ? 0 : (int)RoomType.S2x1L;
                break;
            case RoomType.S2x2:
                ret |= (children[0] == null) ? 0 : (int)RoomType.S2x2U;
                ret |= (children[1] == null) ? 0 : (int)RoomType.S2x2U;
                ret |= (children[2] == null) ? 0 : (int)RoomType.S2x2R;
                ret |= (children[3] == null) ? 0 : (int)RoomType.S2x2R;
                ret |= (children[4] == null) ? 0 : (int)RoomType.S2x2D;
                ret |= (children[5] == null) ? 0 : (int)RoomType.S2x2D;
                ret |= (children[6] == null) ? 0 : (int)RoomType.S2x2L;
                ret |= (children[7] == null) ? 0 : (int)RoomType.S2x2L;
                break;
            case RoomType.S3x3:
                ret |= (children[3] == null) ? 0 : (int)RoomType.S3x3R;
                ret |= (children[4] == null) ? 0 : (int)RoomType.S3x3R;
                ret |= (children[5] == null) ? 0 : (int)RoomType.S3x3R;
                if (ret != (int)RoomType.S3x3R)
                    ret = (int)RoomType.S3x3L;
                break;
            case RoomType.S4x4:
                ret |= (children[4] == null) ? 0 : (int)RoomType.S4x4R;
                ret |= (children[5] == null) ? 0 : (int)RoomType.S4x4R;
                ret |= (children[6] == null) ? 0 : (int)RoomType.S4x4R;
                ret |= (children[7] == null) ? 0 : (int)RoomType.S4x4R;
                if (ret != (int)RoomType.S4x4R)
                    ret = (int)RoomType.S4x4L;
                break;
            default: throw new System.Exception("Wrong Room Type");
        }
        return (RoomType)ret;
    }
}
