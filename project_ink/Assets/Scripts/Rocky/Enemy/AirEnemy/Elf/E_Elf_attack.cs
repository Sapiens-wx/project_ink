using UnityEngine;
using System.Collections;

public class E_Elf_attack : StateBase<E_Elf>{
    const float randBulletPosRange=.5f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet(EnemyBulletManager.inst.elf);
        bullet.transform.position=RandBulletPos();
        bullet.onTriggerEnter+=BulletTriggerEnter;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
    }
    void BulletTriggerEnter(EnemyBulletBase bullet, Collider2D collider){
        bullet.onTriggerEnter-=BulletTriggerEnter;
        ctrller.animator.SetTrigger("idle");
    }
    Vector2 DirToPlayer(){
        return (ctrller.rgb.position-(Vector2)PlayerShootingController.inst.transform.position).normalized;
    }
    Vector2 RandBulletPos(){
        return new Vector2(Random.Range(-randBulletPosRange, randBulletPosRange),0)+(Vector2)ctrller.transform.position;
    }
}