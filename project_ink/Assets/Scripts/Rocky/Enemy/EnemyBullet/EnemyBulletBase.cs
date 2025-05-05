using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    public float spd;
    public int damage=1;
    /// <summary>
    /// auto destroy this bullet after [destroyTime] seconds
    /// </summary>
    [SerializeField] float destroyTime=10;
    [NonSerialized][HideInInspector] public Rigidbody2D rgb;
    [NonSerialized][HideInInspector] public Collider2D bc;
    public event System.Action<EnemyBulletBase,Collider2D> onTriggerEnter;
    bool initialized;
    internal virtual void Start(){
        if(initialized) return;
        initialized=true;
        rgb=GetComponent<Rigidbody2D>();
        bc=GetComponent<Collider2D>();
        Destroy(gameObject, destroyTime);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collider){
        onTriggerEnter?.Invoke(this, collider);
        StartCoroutine(DelayDestroy());
    }
    protected void InvokeOnTriggerEnterEvent(Collider2D collider){
        onTriggerEnter?.Invoke(this, collider);
    }
    protected IEnumerator DelayDestroy(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        yield return wait;
        yield return wait;
        Destroy(gameObject);
    }
    public void UpdateRotation(Vector2 dir){
        float angle=Vector2.SignedAngle(Vector2.up, dir);
        transform.eulerAngles=new Vector3(0,0,angle);
    }
}
