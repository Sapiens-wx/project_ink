using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerTentacle : MonoBehaviour
{
    [SerializeField] Tentacle prefab;
    /// <summary>
    /// amount of time between two consecutive attacks
    /// </summary>
    public float attackInterval;

    public event System.Action<EnemyBase> onHitEnemy;
    ObjectPool<Tentacle> tentaclePool;
    /// <summary>
    /// queue that stores attacks <pos,damage,index>
    /// </summary>
    Queue<Tuple<Vector2,int,int>> attackQ;
    /// <summary>
    /// if [attackAnimIndex] is empty, add 0,1,2 in random order. If not empty, dequeue a number
    /// </summary>
    int attackAnimIndex;
    float lastAttackTime;
    // Start is called before the first frame update
    void Start()
    {
        tentaclePool=new ObjectPool<Tentacle>(Tentacle_Create, Tentacle_Get, Tentacle_Release, Tentacle_Destroy);
        attackQ=new Queue<Tuple<Vector2, int, int>>();
        attackAnimIndex=0;
    }
    void FixedUpdate(){
        //attack
        if(attackQ.Count>0 && Time.time>lastAttackTime+attackInterval){
            lastAttackTime=Time.time;
            var atkinfo=attackQ.Dequeue();
            Tentacle tentacle=tentaclePool.Get();
            tentacle.Attack(atkinfo.Item1, atkinfo.Item2, atkinfo.Item3);
        }
    }
    public void Attack(Vector2 point, int _damage){
        attackQ.Enqueue(new Tuple<Vector2, int, int>(point,_damage,attackAnimIndex++));
        if(attackAnimIndex>2) attackAnimIndex=0;
    }
    #region Pool Utilities
    Tentacle Tentacle_Create(){
        Tentacle res=Instantiate(prefab.gameObject, PlayerCtrl.inst.transform).GetComponent<Tentacle>();
        res.transform.localPosition=Vector3.zero;
        res.onHitEnemy+=onHitEnemy;
        return res;
    }
    void Tentacle_Get(Tentacle te){
        te.gameObject.SetActive(true);
        te.onAttackEnd+=AutoRelease;
    }
    void Tentacle_Release(Tentacle t){
        t.gameObject.SetActive(false);
        t.onAttackEnd-=AutoRelease;
    }
    static void Tentacle_Destroy(Tentacle t){
        if(t!=null) Destroy(t);
    }
    void AutoRelease(Tentacle te){
        tentaclePool.Release(te);
    }
    #endregion
}