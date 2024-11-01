using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] ProgressBar healthBar;
    [SerializeField] internal int maxHealth;
    int curHealth;
    internal int CurHealth{
        get=>curHealth;
        set{
            curHealth=value;
            healthBar.SetProgress((float)curHealth/maxHealth);
        }
    }
    [HideInInspector] public int id; //id in a room
    public abstract void OnHit(Projectile proj);
    internal virtual void Start(){
        RoomManager.inst.RegisterEnemy(this);
        CurHealth=maxHealth;
    }
    void OnDestroy(){
        RoomManager.inst.UnRegisterEnemy(this);
    }
}
