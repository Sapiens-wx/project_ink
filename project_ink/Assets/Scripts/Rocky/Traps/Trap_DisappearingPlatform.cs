using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Trap_DisappearingPlatform : TrapBase
{
    [SerializeField] float duration, disappearDuration;

    Collider2D bc;
    Vector2 rayOrigin;
    float rayLength;
    bool trapActivated;
    void Awake(){
        bc=sprite.GetComponent<Collider2D>();
        rayOrigin=new Vector2(bc.bounds.min.x,bc.bounds.max.y+MathUtil.colliderMinGap);
        rayLength=bc.bounds.size.x;
    }
    void FixedUpdate(){
        if(!trapActivated){
            RaycastHit2D hit=Physics2D.Raycast(rayOrigin,Vector2.right,rayLength, GameManager.inst.playerLayer);
            //if player enters the trap for the first time
            if(hit){
                trapActivated=true;
                StartCoroutine(Disappear());
            }
        }
    }
    IEnumerator Disappear(){
        yield return new WaitForSeconds(duration);
        sprite.gameObject.SetActive(false);
        yield return new WaitForSeconds(disappearDuration);
        sprite.gameObject.SetActive(true);
        trapActivated=false;
    }
}
