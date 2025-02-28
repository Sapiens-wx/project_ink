using UnityEngine;

public class E_Goat_attack_fly : StateBase<E_Goat_Ground>{
    float targetHeight;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        SetTargetHeight();
        ctrller.rgb.gravityScale=0;
        ctrller.rgb.velocity=new Vector2(0, ctrller.flySpd);
        ctrller.balloon.gameObject.SetActive(true);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(ctrller.transform.position.y>=targetHeight)
            animator.SetTrigger("attack");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        ctrller.rgb.velocity=new Vector2(ctrller.rgb.velocity.x,0);
    }
    void SetTargetHeight(){
        RaycastHit2D hit_down = Physics2D.Raycast(ctrller.transform.position, Vector2.down, float.MaxValue, GameManager.inst.groundMixLayer);
        RaycastHit2D hit_up = Physics2D.Raycast(ctrller.transform.position, Vector2.up, float.MaxValue, GameManager.inst.groundMixLayer);
        if(!hit_down){
            Debug.LogWarning("Goat attack_fly: ray didn't hit ground");
            targetHeight=ctrller.flyHeight+ctrller.transform.position.y;
        } else
            targetHeight=ctrller.flyHeight+hit_down.point.y;
        if(hit_up) targetHeight=Mathf.Min(targetHeight, hit_up.point.y-.025f-ctrller.bc.bounds.extents.y+ctrller.bc.offset.y);
    }
}