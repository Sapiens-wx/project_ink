using UnityEngine;

public class E_Monkey_attack : StateBase<E_Monkey>{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.monkey, ctrller.transform.position, PlayerShootingController.inst.transform.position-ctrller.transform.position);
    }
}