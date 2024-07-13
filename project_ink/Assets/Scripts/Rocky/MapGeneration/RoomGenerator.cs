using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

/// <summary>
/// Generate a tree of rooms. Does not generate things inside the rooms
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("Generate Map")]
    public GameObject img_prefab;
    public Sprite[] mapTiles;
    public int mapWidth, mapHeight;
    public Sprite spr_door;
    [Range(0f, 1f)]
    public float doorRatio; //ratio to the size of a grid. 0-1
    public Transform mapContainer;
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
            n = q.Count();
            ++count;
        }
    }
    public void GenerateRoomMap(Room root, Transform parent)
    {
        float wFactor = mapWidth / 23.0f;
        float hFactor = mapHeight / 45.0f;
        float doorWidth = wFactor * doorRatio;

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
            img.sprite = mapTiles[(curRoom.w << 1) + curRoom.h - 3];
            go.transform.position = new Vector3(curRoom.x * wFactor, curRoom.y * hFactor);
            center += go.transform.position; //add this room's position to center (Vector3), then calculate actual center after the loop
            rectTransform.sizeDelta = new Vector2(curRoom.w*wFactor, curRoom.h*hFactor);
            for(int i = 0; i < curRoom.children.Length; ++i)
            {
                if (curRoom.children[i] != null)
                    q.Enqueue(curRoom.children[i]);
            }
            ++roomsCount;
        }
        center = center / roomsCount - parent.position;
        for(int i = 0; i<parent.childCount; ++i)
        {
            parent.GetChild(i).position -= center; //make the rooms images center around their parent
        }
    }
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
        RandomlyEnqueue(q, Door.GetDoorsFromRoom(root, 3));
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
}
// how is the size and door position matched to a binary number?
// width[00]height[00]up[0]right[0]down[0]left[0]
// exampmle: 1x2 width up, right, down doors: 01 10 1 1 1 0
/// <summary>
/// based on the size of the room and the number and position of its doors
/// </summary>
public enum RoomType
{
    S1x1URD=0b01011110,
    S1x1RDL=0b01010111,
    S1x1UDL=0b01011011,
    S1x1URL=0b01011101,
    S1x1UR=0b01011100,
    S1x1UD=0b01011010,
    S1x1UL=0b01011001,
    S1x1RD=0b01010110,
    S1x1RL=0b01010101,
    S1x1DL=0b01010011,
    S1x1U=0b01011000,
    S1x1R=0b01010100,
    S1x1D=0b01010010,
    S1x1L=0b01010001,

    S1x2URD=0b01101110,
    S1x2RDL=0b01100111,
    S1x2UDL=0b01101011,
    S1x2URL=0b01101101,
    S1x2UR=0b01101100,
    S1x2UD=0b01101010,
    S1x2UL=0b01101001,
    S1x2RD=0b01100110,
    S1x2RL=0b01100101,
    S1x2DL=0b01100011,
    S1x2U=0b01101000,
    S1x2R=0b01100100,
    S1x2D=0b01100010,
    S1x2L=0b01100001,

    S2x1URD=0b10011110,
    S2x1RDL=0b10010111,
    S2x1UDL=0b10011011,
    S2x1URL=0b10011101,
    S2x1UR=0b10011100,
    S2x1UD=0b10011010,
    S2x1UL=0b10011001,
    S2x1RD=0b10010110,
    S2x1RL=0b10010101,
    S2x1DL=0b10010011,
    S2x1U=0b10011000,
    S2x1R=0b10010100,
    S2x1D=0b10010010,
    S2x1L=0b10010001,

    S2x2URD=0b10101110,
    S2x2RDL=0b10100111,
    S2x2UDL=0b10101011,
    S2x2URL=0b10101101,
    S2x2UR=0b10101100,
    S2x2UD=0b10101010,
    S2x2UL=0b10101001,
    S2x2RD=0b10100110,
    S2x2RL=0b10100101,
    S2x2DL=0b10100011,
    S2x2U=0b10101000,
    S2x2R=0b10100100,
    S2x2D=0b10100010,
    S2x2L=0b10100001,
    //debug usage, without doors, don't use when configuring rooms
    S1x1=0b01010000,
    S1x2=0b01100000,
    S2x1=0b10010000,
    S2x2=0b10100000
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
                    l = door.x; r = l + w;
                    b = door.y - 1; t = b + h;
                    if (w == 2) {
                        --l;--r;
                    }
                    break;
                case 1: //right
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    if (h == 2) {
                        ++t;++b;
                    }
                    break;
                case 2: //down
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    if (w == 2) {
                        --l;--r;
                    }
                    break;
                case 3: //left
                    r = door.x + 1; l = r - w;
                    t = door.y; b = t - h;
                    if (h == 2) {
                        ++t;++b;
                    }
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
                doors = new Door[4];
                for (int i = 0; i < 4; ++i)
                    doors[i] = new Door(this, i, i);
                doors[0].SetPosition(x, y + 1);
                doors[1].SetPosition(x + 1, y);
                doors[2].SetPosition(x, y - 1);
                doors[3].SetPosition(x - 1, y);
            }
            else {
                doors = new Door[4];
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
                doors = new Door[4];
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
                doors = new Door[4];
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
    public RoomType GetRoomtType()
    {
        int ret = (w << 6) | (h << 4);
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
            default: throw new System.Exception("Wrong Room Type");
        }
        return (RoomType)ret;
    }
}
