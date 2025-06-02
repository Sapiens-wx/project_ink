using UnityEngine;
using DG.Tweening;

public class Trap_MovingPlatform : TrapBase
{
    [SerializeField] float spd;
    [SerializeField] Vector2 start,end;
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
}
