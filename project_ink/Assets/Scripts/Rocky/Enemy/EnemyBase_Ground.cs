using UnityEngine;

public abstract class EnemyBase_Ground : MobBase
{
    [Header("Attack Detection")]
    public Bounds attackTriggerBounds;
    [Header("patrol")]
    public float walkSpd;
    /// <summary>
    /// the patrol min/max relative to the pivot of the enemy object
    /// </summary>
    [HideInInspector] public float patrol_xmin, patrol_xmax;

    //ground detection
    [HideInInspector] public bool onGround, prevOnGround;
    internal override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(attackTriggerBounds.center+transform.position, attackTriggerBounds.size);
        Gizmos.color=Color.blue;
    }
    /// <summary>
    /// used to determine the bounds of the platform when the enemy is in patrol state
    /// </summary>
    public void UpdateGroundXMinMax(){
        Bounds bounds=bc.bounds;
        Collider2D ground=Physics2D.OverlapArea(bounds.min,bounds.max,GameManager.inst.groundLayer);
        bounds.center=new Vector3(bounds.center.x, ground.bounds.max.y+bounds.extents.y, 0);
        Vector2 leftBot=bounds.min;
        Vector2 rightBot=leftBot;
        rightBot.x+=bounds.size.x;
        Vector2 leftTop=leftBot;
        leftTop.y+=bounds.size.y;
        Vector2 rightTop=bounds.max;
        if(ground==null) return;
        patrol_xmin=ground.bounds.min.x;
        patrol_xmax=ground.bounds.max.x;
        //check if there is any obstacles
        Vector2 offset=new Vector2(0,.1f);
        RaycastHit2D leftTopHit=Physics2D.Raycast(leftTop-offset, Vector2.left, Mathf.Clamp(leftTop.x-patrol_xmin,0,float.MaxValue), GameManager.inst.groundLayer);
        RaycastHit2D leftBotHit=Physics2D.Raycast(leftBot+offset, Vector2.left, Mathf.Clamp(leftTop.x-patrol_xmin,0,float.MaxValue), GameManager.inst.groundLayer);
        RaycastHit2D rightTopHit=Physics2D.Raycast(rightTop-offset, Vector2.right, Mathf.Clamp(patrol_xmax-rightTop.x,0,float.MaxValue), GameManager.inst.groundLayer);
        RaycastHit2D rightBotHit=Physics2D.Raycast(rightBot+offset, Vector2.right, Mathf.Clamp(patrol_xmax-rightTop.x,0,float.MaxValue), GameManager.inst.groundLayer);
        if(leftTopHit==true) patrol_xmin=leftTopHit.point.x;
        if(leftBotHit==true) patrol_xmin=patrol_xmin<leftBotHit.point.x?leftBotHit.point.x:patrol_xmin;
        if(rightTopHit==true) patrol_xmax=rightTopHit.point.x;
        if(rightBotHit==true) patrol_xmax=patrol_xmax>rightBotHit.point.x?patrol_xmax:rightBotHit.point.x;
        //calculate patrol min/max relative to the pivot of the enemy gameobject.
        patrol_xmin+=bounds.extents.x+.1f; //.1f is the padding
        patrol_xmax-=bounds.extents.x+.1f;
    }
    internal void CheckOnGround(){
        prevOnGround=onGround;
        Bounds bounds=bc.bounds;
        Vector2 leftBot=bounds.min;
        Vector2 rightBot=leftBot;
        rightBot.x+=bounds.size.x*.9f;
        leftBot.x+=bounds.size.x*.1f;
        onGround = Physics2D.OverlapArea(leftBot,rightBot,GameManager.inst.groundLayer);
    }
    internal override void FixedUpdate(){
        base.FixedUpdate();
        //attack trigger detection
        prevPlayerInAttack=playerInAttack;
        playerInAttack=PlayerInRange(attackTriggerBounds);
        if(playerInAttack&&!prevPlayerInAttack){ //on detect enter
            animator.SetBool("b_attack", true);
        } else if(!playerInAttack&&prevPlayerInAttack) //on detect exit
            animator.SetBool("b_attack",false);

        CheckOnGround();
        if(!prevOnGround && onGround){ //landing
            UpdateGroundXMinMax();
            animator.SetBool("b_onground", true);
        } else if(!onGround && prevOnGround) //leave the ground
            animator.SetBool("b_onground", false);
    }
}
