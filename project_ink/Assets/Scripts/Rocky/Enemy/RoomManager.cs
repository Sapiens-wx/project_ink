using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [SerializeField] private Bounds roomBounds;

    public static RoomManager CurrentRoom{
        get=>inst;
    }
    public Bounds RoomBounds{
        get=>roomBounds;
    }

    void OnDrawGizmosSelected(){
        Gizmos.color=Color.green;
        Gizmos.DrawWireCube(roomBounds.center, roomBounds.extents*2);
    }
}
