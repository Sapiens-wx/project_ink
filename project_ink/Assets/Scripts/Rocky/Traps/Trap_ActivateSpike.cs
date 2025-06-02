using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Trap_ActivateSpike : TrapBase
{
    [SerializeField] float length, activateLatency, activateDuration;
    [SerializeField] Vector2 moveToPos;

    Vector2 rayOrigin;
    bool trapActivated;
    void OnValidate(){
        rayOrigin=(Vector2)transform.position+new Vector2(-length/2,MathUtil.colliderMinGap);
    }
    void OnDrawGizmosSelected(){
        Gizmos.DrawLine(rayOrigin, rayOrigin+new Vector2(length,0));
        Gizmos.DrawWireSphere((Vector2)transform.position+moveToPos,.3f);
    }
    override protected void Start(){
        base.Start();
        OnValidate();
    }
    void FixedUpdate(){
        if(!trapActivated){
            RaycastHit2D hit=Physics2D.Raycast(rayOrigin,Vector2.right,length,GameManager.inst.playerLayer);
            //if player enters the trap for the first time
            if(hit){
                trapActivated=true;
                StartCoroutine(ShowSpike());
            }
        }
    }
    IEnumerator ShowSpike(){
        yield return new WaitForSeconds(activateLatency);
        activeSprite.transform.DOMove((Vector2)transform.position+moveToPos, activateDuration);
    }
}
