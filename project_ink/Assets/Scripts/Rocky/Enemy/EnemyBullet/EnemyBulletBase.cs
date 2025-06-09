using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : EnemyDamageBox
{
    public float spd;
    /// <summary>
    /// auto destroy this bullet after [destroyTime] seconds
    /// </summary>
    [SerializeField] float destroyTime=10;
    [NonSerialized][HideInInspector] public Collider2D bc;
    public event System.Action<EnemyBulletBase,Collider2D> onTriggerEnter;

    [NonSerialized][HideInInspector] public Vector2 velocity, trap_v_fan;
    public float AngularVelocity{
        get=>rgb.angularVelocity;
        set=>rgb.angularVelocity=value;
    }
    Rigidbody2D rgb;
    [NonSerialized][HideInInspector] public float gravity;
    void Awake(){
        rgb=GetComponent<Rigidbody2D>();
        bc=GetComponent<Collider2D>();
        gravity=9.8f*Time.fixedDeltaTime*rgb.gravityScale;
        rgb.bodyType=RigidbodyType2D.Kinematic;
    }
    internal virtual void Start(){
        Destroy(gameObject, destroyTime);
    }
    protected virtual void FixedUpdate(){
        velocity.y-=rgb.gravityScale*9.8f*Time.fixedDeltaTime; //apply gravity
        rgb.velocity=velocity+trap_v_fan;
    }
    protected virtual void OnTriggerEnter2D(Collider2D collider){
        onTriggerEnter?.Invoke(this, collider);
        if(GameManager.IsLayer(GameManager.inst.enemyBulletDestroyLayer, collider.gameObject.layer)){
            StartCoroutine(DelayDestroy());
        }
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
