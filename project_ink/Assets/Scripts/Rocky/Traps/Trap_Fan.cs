using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Trap_Fan : TrapBase
{
    [SerializeField] float fanSpd, fanAngle; //fanAngle is in degree

    Vector2 fanDir;
    Collider2D bc;
    LayerMask layerMask;
    Collider2D[] overlapColliders, prevOverlapColliders;
    void OnValidate(){
        fanDir=MathUtil.Rad2Dir(Mathf.Deg2Rad*fanAngle);
    }
    protected override void Start()
    {
        base.Start();
        OnValidate();
        layerMask=GameManager.GetLayerMask(GameManager.inst.playerLayer,GameManager.inst.enemyLayer,GameManager.inst.enemyBulletLayer,GameManager.inst.projectileLayer);
    }
    public override void ChangeTheme(Theme theme)
    {
        base.ChangeTheme(theme);
        bc=activeSprite.GetComponent<Collider2D>();
    }
    void OnDrawGizmosSelected(){
        Gizmos.color=Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position+fanDir);
    }
    void FixedUpdate(){
        CheckCollisions();
    }
    void CheckCollisions(){
        if(bc!=null){
            Bounds bd=bc.bounds;
            overlapColliders=Physics2D.OverlapAreaAll(bd.min,bd.max,layerMask);
            if(prevOverlapColliders==null){
                if(overlapColliders!=null)
                    foreach(var cd in overlapColliders)
                        OnTriggerEnter2D(cd);
            } else if(overlapColliders==null){
                foreach(var cd in prevOverlapColliders)
                    OnTriggerExit2D(cd);
            } else{
                for(int i=overlapColliders.Length-1;i>-1;--i){
                    if(!prevOverlapColliders.Contains(overlapColliders[i]))
                        OnTriggerEnter2D(overlapColliders[i]);
                }
                for(int i=prevOverlapColliders.Length-1;i>-1;--i){
                    if(!overlapColliders.Contains(prevOverlapColliders[i]))
                        OnTriggerExit2D(prevOverlapColliders[i]);
                }
            }
            prevOverlapColliders=overlapColliders;
        }
    }
    void OnTriggerEnter2D(Collider2D collider){
        int layer=collider.gameObject.layer;
        //if is player
        if(GameManager.IsLayer(GameManager.inst.playerLayer, layer)){
            PlayerCtrl.inst.v_trap_fan+=fanDir*fanSpd;
        }//if is enemy
        else if(GameManager.IsLayer(GameManager.inst.enemyLayer, layer)&&
            collider.GetComponent<EnemyBase_Air>()==null){
            Rigidbody2D rgb=collider.attachedRigidbody;
            //rgb.velocity=new Vector2(rgb.velocity.x, Mathf.Sqrt(19.6f*height));
        }//if is projectile
        else if(GameManager.IsLayer(GameManager.inst.projectileLayer, layer)){
            collider.GetComponent<Projectile>().trap_v_fan+=fanDir*fanSpd;
        }//if is enemy bullet
        else if(GameManager.IsLayer(GameManager.inst.enemyBulletLayer, layer)){
            //the collider might be from enemy damage box, which is in enemy bullet layer but does not have enemy bullet base component
            EnemyBulletBase bullet=collider.GetComponent<EnemyBulletBase>();
            if(bullet)
                bullet.trap_v_fan+=fanDir*fanSpd;
        }
    }
    void OnTriggerExit2D(Collider2D collider){
        int layer=collider.gameObject.layer;
        //if is player or enemy
        if(GameManager.IsLayer(GameManager.inst.playerLayer, layer)){
            PlayerCtrl.inst.v_trap_fan-=fanDir*fanSpd;
        }//if is enemy
        else if(GameManager.IsLayer(GameManager.inst.enemyLayer, layer)&&
            collider.GetComponent<EnemyBase_Air>()==null){
            Rigidbody2D rgb=collider.attachedRigidbody;
            //rgb.velocity=new Vector2(rgb.velocity.x, Mathf.Sqrt(19.6f*height));
        }//if is projectile
        else if(GameManager.IsLayer(GameManager.inst.projectileLayer, layer)){
            collider.GetComponent<Projectile>().trap_v_fan-=fanDir*fanSpd;
        }//if is enemy bullet
        else if(GameManager.IsLayer(GameManager.inst.enemyBulletLayer, layer)){
            EnemyBulletBase bullet=collider.GetComponent<EnemyBulletBase>();
            if(bullet)
                bullet.trap_v_fan+=fanDir*fanSpd;
        }
    }
}