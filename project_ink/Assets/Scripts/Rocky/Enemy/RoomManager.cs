using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [SerializeField] private Bounds roomBounds;
    [Header("Enemy")]
    [SerializeField] EnemyList enemyPrefabList;

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
        enemyPrefabList.Init();
        enemies=new List<EnemyBase>();
    }
    public int RegisterEnemy(EnemyBase enemy){
        enemy.id=enemies.Count;
        enemies.Add(enemy);
        return enemy.id;
    }
    public void UnRegisterEnemy(EnemyBase enemy){
        enemies[enemy.id]=enemies[^1];
        enemies[^1].id=enemy.id;
        enemies.RemoveAt(enemies.Count-1);
    }
    public EnemyBase ClosestEnemy(Transform target){
        if(enemies.Count==0) return null;
        float min=float.MaxValue;
        EnemyBase minEnemy=null;
        foreach(EnemyBase enemy in enemies){
            Vector2 distv=target.transform.position-enemy.transform.position;
            float dist=distv.x*distv.x+distv.y*distv.y;
            if(dist<min){
                min=dist;
                minEnemy=enemy;
            }
        }
        return minEnemy;
    }
    /// <summary>
    /// enemies include boss and mobs
    /// </summary>
    public List<EnemyBase> EnemiesInRange(Vector2 center, float dist){
        List<EnemyBase> ret=new List<EnemyBase>();
        dist*=dist;
        foreach(EnemyBase e in enemies){
            Vector2 dir=(Vector2)e.transform.position-center;
            float d=dir.x*dir.x+dir.y*dir.y;
            if(d<=dist){
                ret.Add(e);
            }
        }
        return ret;
    }
    /// <summary>
    /// finds only mobs
    /// </summary>
    public List<MobBase> MobsInRange(Vector2 center, float dist){
        List<MobBase> ret=new List<MobBase>();
        dist*=dist;
        foreach(EnemyBase e in enemies){
            MobBase m=e as MobBase;
            if(m==null) continue;
            Vector2 dir=(Vector2)e.transform.position-center;
            float d=dir.x*dir.x+dir.y*dir.y;
            if(d<=dist){
                ret.Add(m);
            }
        }
        return ret;
    }
    //can be optimized
    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if there is a corresponding enemy in the room</returns>
    public bool HasEnemy(System.Predicate<EnemyBase> predicate){
        foreach(EnemyBase e in enemies){
            if(predicate(e)) return true;
        }
        return false;
    }
    /// <summary>
    /// activate all elite enemies. Called when the player enters a room
    /// </summary>
    void ActivateElites(){
        foreach(EnemyBase e in enemies){
            EliteBase eg=e as EliteBase;
            if(eg==null) continue;
            eg.Activate();
        }
    }
    public EnemyBase InstantiateEnemy(EnemyType enemyType){
        EnemyBase e=Instantiate(enemyPrefabList[enemyType]).GetComponent<EnemyBase>();
        e.Start();
        return e;
    }
}
