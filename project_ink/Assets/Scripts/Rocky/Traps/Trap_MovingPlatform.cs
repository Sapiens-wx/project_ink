using UnityEngine;
using DG.Tweening;

public class Trap_MovingPlatform : TrapBase
{
    [SerializeField] float spd;
    [SerializeField] Vector2 start,end;

    Collider2D bc;
    Vector2 prevPos;
    bool prevHit;
    void OnDrawGizmosSelected(){
        Gizmos.color=Color.green;
        Gizmos.DrawLine(start,end);
    }
    protected override void Start(){
        base.Start();
        transform.position=start;
        Sequence s=DOTween.Sequence();
        float dist=Vector2.Distance(start,end);
        transform.DOMove(end,dist/spd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    void FixedUpdate(){
        //ray origin: top left of the bounds. distance: bounds.size.x
        Vector2 topLeft=new Vector2(bc.bounds.min.x,bc.bounds.max.y+MathUtil.colliderMinGap);
        RaycastHit2D hit=Physics2D.Raycast(topLeft,Vector2.right,bc.bounds.size.x,GameManager.inst.playerLayer);
        Vector2 velocity=((Vector2)transform.position-prevPos)/Time.fixedDeltaTime;
        //if player enters on the platform, activate the platform
        if(!hit && prevHit){ //player exits platform
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
