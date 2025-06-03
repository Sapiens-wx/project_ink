using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_1_A5_Bullet : EnemyBulletBase
{
    public float bullet_angle;
    protected override void OnTriggerEnter2D(Collider2D collider){
        InvokeOnTriggerEnterEvent(collider);
        //if is going upward and touches the ground, do not destroy
        if(!(GameManager.IsLayer(GameManager.inst.groundLayer, collider.gameObject.layer) && velocity.y>0)){
            StartCoroutine(DelayDestroy());
            Vector3 bulletPos=transform.position;
            bulletPos.y=collider.bounds.max.y+bc.bounds.extents.y*1.2f;
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,Vector2.up);
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,MathUtil.Rotate(Vector2.up,bullet_angle*Mathf.Deg2Rad));
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,MathUtil.Rotate(Vector2.up,-bullet_angle*Mathf.Deg2Rad));
        }
    }
}
