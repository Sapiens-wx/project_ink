using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] internal int maxHealth;
    internal int curHealth;
    [HideInInspector] public int id; //id in a room
    public abstract void OnHit(Projectile proj);
    internal virtual void Start(){
        RoomManager.inst.RegisterEnemy(this);
    }
    void OnDestroy(){
        RoomManager.inst.UnRegisterEnemy(this);
    }
}
