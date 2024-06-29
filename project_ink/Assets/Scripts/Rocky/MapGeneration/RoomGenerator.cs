using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RoomGenerator : MonoBehaviour
{
    [Header("Generate Map")]
    public Sprite[] mapTiles;
    public int mapWidth, mapHeight;
    public GameObject mapTilePrefab;
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
    public void GenerateRoomMap(Room root, Transform parent)
    {
        float wFactor = mapWidth / 23.0f;
        float hFactor = mapHeight / 45.0f;
        Queue<Room> q = new Queue<Room>();
        Room curRoom;
        q.Enqueue(root);
        while (q.Count > 0)
        {
            curRoom = q.Dequeue();
            //generate map
            GameObject go = GameObject.Instantiate(mapTilePrefab, mapContainer);
            go.name = $"{curRoom.w}x{curRoom.h} ({curRoom.x},{curRoom.y})";
            Image img = go.GetComponent<Image>();
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            img.sprite = mapTiles[(curRoom.w << 1) + curRoom.h - 3];
            go.transform.position = new Vector3(curRoom.x * wFactor, curRoom.y * hFactor);
            rectTransform.sizeDelta = new Vector2(curRoom.w*wFactor, curRoom.h*hFactor);
            for(int i = 0; i < curRoom.children.Length; ++i)
            {
                if (curRoom.children[i] != null)
                    q.Enqueue(curRoom.children[i]);
            }
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
            RandomlyEnqueue(q, Door.GetDoorsFromRoom(selectedRoom, position.z));
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
    private void SetPosition(int _x, int _y)
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
                ret = new Door[6];
                ret[0] = new Door(room, 0, room.x, room.y+2,0);
                ret[1] = new Door(room, 1, room.x+1, room.y+1,1);
                ret[2] = new Door(room, 1, room.x+1, room.y,2);
                ret[3] = new Door(room, 2, room.x, room.y-1,3);
                ret[4] = new Door(room, 3, room.x-1, room.y,4);
                ret[5] = new Door(room, 3, room.x-1, room.y+1,5);
            }
        }
        else {
            if (room.h == 2) {
                ret = new Door[8];
                for (int i = 0; i < 8; ++i)
                    ret[i] = new Door(room, i >> 1, i);
                ret[0].SetPosition(room.x, room.y+2);
                ret[1].SetPosition(room.x+1, room.y+2);
                ret[2].SetPosition(room.x+2, room.y+1);
                ret[3].SetPosition(room.x+2, room.y);
                ret[4].SetPosition(room.x+1, room.y-1);
                ret[5].SetPosition(room.x, room.y-1);
                ret[6].SetPosition(room.x-1, room.y);
                ret[7].SetPosition(room.x-1, room.y+1);
            }
            else {
                ret = new Door[6];
                ret[0] = new Door(room, 0, room.x, room.y+1,0);
                ret[1] = new Door(room, 0, room.x+1, room.y+1,1);
                ret[2] = new Door(room, 1, room.x+2, room.y,2);
                ret[3] = new Door(room, 2, room.x+1, room.y-1,3);
                ret[4] = new Door(room, 2, room.x, room.y-1,4);
                ret[5] = new Door(room, 3, room.x-1, room.y,5);
            }
        }
        return DeleteFromArray(ret, except);
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
public class Room
{
    public int w, h;
    /// <summary>
    /// 0: up, 1: right, 2: down, 3: left
    /// </summary>
    public Room[] children;
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
                    position.z = (w << 1) + h - 1;
                    break;
                case 1: //right
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    position.z = (w << 1) + (h << 1) - 1;
                    break;
                case 2: //down
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    position.z = 0;
                    break;
                case 3: //left
                    r = door.x + 1; l = r - w;
                    t = door.y; b = t - h;
                    position.z = w;
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
                    position.z = w + h;
                    break;
                case 1: //right
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    if (h == 2) {
                        ++t;++b;
                    }
                    position.z = (w << 1) + h;
                    break;
                case 2: //down
                    l = door.x; r = l + w;
                    t = door.y; b = t - h;
                    if (w == 2) {
                        --l;--r;
                    }
                    position.z = w - 1;
                    break;
                case 3: //left
                    r = door.x + 1; l = r - w;
                    t = door.y; b = t - h;
                    if (h == 2) {
                        ++t;++b;
                    }
                    position.z = w + h - 1;
                    break;
                default: return false;
            }
        }
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
}
