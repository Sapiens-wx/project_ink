using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [SerializeField] private Bounds roomBounds;
    [Header("Enemy")]
    [SerializeField] EnemyList enemyPrefabList;

    [HideInInspector] public List<EnemyBase> enemies;

    static RoomManager currentRoom;
    public static RoomManager CurrentRoom{
        get=>currentRoom;
        private set{currentRoom=value; if(value!=null) currentRoom.OnEnterRoom();}
    }
    public Bounds RoomBounds{
        get{
            Bounds b=roomBounds;
            b.center+=transform.position;
            return b;
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color=Color.green;
        Gizmos.DrawWireCube(transform.position+roomBounds.center, roomBounds.extents*2);
    }
    internal override void Awake()
    {
        base.Awake();
        enemyPrefabList.Init();
        enemies=new List<EnemyBase>();
    }
    void Start(){
        ActivateElites();
        StartCoroutine(CheckCurrentRoom());
    }
    #region CurrentRoom
    void OnEnterRoom(){
        TentacleManager.inst.canReconcile=false;
    }
    IEnumerator CheckCurrentRoom(){
        WaitForSeconds wait=new WaitForSeconds(.3f);
        while(true){
            //when player is not in any room, check which room player is in
            if(currentRoom==null && PlayerInsideRoom())
                CurrentRoom=this;
            //when player is inside a room, check whether player is inside the room
            else if(currentRoom==this && !PlayerInsideRoom())
                CurrentRoom=null;
            yield return wait;
        }
    }
    bool PlayerInsideRoom(){
        Vector2 min=roomBounds.min, max=roomBounds.max;
        Vector2 pos=PlayerCtrl.inst.transform.position;
        return pos.x>=min.x && pos.x<=max.x && pos.y>=min.y && pos.y<=max.y;
    }
    #endregion
    #region Enemy
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
            if(dist<min && enemy.gameObject.activeSelf){
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
    #endregion
}
