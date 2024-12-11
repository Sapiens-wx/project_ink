using UnityEngine;

public class E_Frog_attack : StateBase<E_Frog>{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.frog, ctrller.transform.position, 
            ((Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.transform.position).normalized);
        animator.SetTrigger("attack");
    }
}