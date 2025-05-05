using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_1_A4_Bullet : EnemyBulletBase
{
    public float bullet_angle;
    protected override void OnTriggerEnter2D(Collider2D collider){
        InvokeOnTriggerEnterEvent(collider);
        //if is going upward and touches the ground, do not destroy
        StartCoroutine(DelayDestroy());
        Vector2 flyingDir=rgb.velocity.normalized;
        Ray ray=new Ray(transform.position, flyingDir);
        collider.bounds.IntersectRay(ray, out float dist);//{
            flyingDir=-MathUtil.CalculateIntersectionNormal(ray, collider.bounds);
            Vector3 bulletPos=(Vector2)transform.position+flyingDir*.3f;
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,flyingDir);
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,MathUtil.Rotate(flyingDir,bullet_angle*Mathf.Deg2Rad));
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a5_2,bulletPos,MathUtil.Rotate(flyingDir,-bullet_angle*Mathf.Deg2Rad));
        //}
    }
}
