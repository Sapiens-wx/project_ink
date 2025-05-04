using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    public float spd;
    public int damage=1;
    [HideInInspector] public Rigidbody2D rgb;
    public event System.Action<EnemyBulletBase,Collider2D> onTriggerEnter;
    bool initialized;
    internal virtual void Start(){
        if(initialized) return;
        initialized=true;
        rgb=GetComponent<Rigidbody2D>();
        Destroy(gameObject, 10);
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
}
