using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Trap_ActivatePlatform : TrapBase
{
    [SerializeField] float duration;
    [SerializeField] Vector2 start,end;

    Collider2D bc;
    bool prevHit, atStartingPoint;
    Vector2 prevPos;
    void OnDrawGizmosSelected(){
        Gizmos.color=Color.green;
        Gizmos.DrawLine(start,end);
    }
    protected override void Start(){
        base.Start();
        transform.position=start;
        prevPos=start;
        atStartingPoint=true;
    }
    void FixedUpdate(){
        //ray origin: top left of the bounds. distance: bounds.size.x
        Vector2 topLeft=new Vector2(bc.bounds.min.x,bc.bounds.max.y+MathUtil.colliderMinGap);
        RaycastHit2D hit=Physics2D.Raycast(topLeft,Vector2.right,bc.bounds.size.x,GameManager.inst.playerLayer);
        Vector2 velocity=((Vector2)transform.position-prevPos)/Time.fixedDeltaTime;
        //if player enters on the platform, activate the platform
        if(hit&&!prevHit){
            float speed=Vector2.Dot(velocity,velocity);
            //no need to reactivate the platform when it is moving
            if(speed<.01f){
                transform.DOMove(atStartingPoint?end:start, duration).SetEase(Ease.InSine);
                atStartingPoint=!atStartingPoint;
            }
        } else if(!hit && prevHit){ //player exits platform
            PlayerCtrl.inst.v_trap_platform=Vector2.zero;
        }
        //what player stays on the platform, update player.v_trap_platform
        if(hit){
            PlayerCtrl.inst.v_trap_platform=velocity;
        }
        prevHit=hit;
        prevPos=transform.position;
    }
    public override void ChangeTheme(Theme theme)
    {
        base.ChangeTheme(theme);
        bc=activeSprite.GetComponent<Collider2D>();
    }
}