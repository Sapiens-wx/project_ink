using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [SerializeField] private Bounds roomBounds;

    [HideInInspector] public List<EnemyBase> enemies;

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
    internal override void Awake()
    {
        base.Awake();
        enemies=new List<EnemyBase>();
    }
    public int RegisterEnemy(EnemyBase enemy){
        enemy.id=enemies.Count;
        enemies.Add(enemy);
        return enemies.Count-1;
    }
    public void UnRegisterEnemy(EnemyBase enemy){
        enemies[enemy.id]=enemies[^1];
        enemies.RemoveAt(enemies.Count-1);
    }
    public EnemyBase ClosestEnemy(Transform target){
        if(enemies.Count==0) return null;
        return enemies[0];
    }
}
