using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class E_Diamond_attack : StateBase<E_Diamond>{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        float shootAngle=ctrller.shootAngle+Random.Range(-ctrller.shootAngleRange/2, ctrller.shootAngleRange/2);
        float shootForce=ctrller.shootForce+Random.Range(-ctrller.shootForceRange/2, ctrller.shootForceRange/2);
        Vector2 v=new Vector2(ctrller.Dir*shootForce,0);
        //rotate v to get the desired direction
        if(ctrller.Dir==-1) shootAngle=-shootAngle;
        shootAngle*=Mathf.Deg2Rad;
        v=new Vector2(v.x*Mathf.Cos(shootAngle),v.x*Mathf.Sin(shootAngle));
        EnemyBulletManager.InstantiateBullet_v(EnemyBulletManager.inst.diamond, ctrller.transform.position, v);
    }
}