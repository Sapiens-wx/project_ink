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
    public override void ChangeTheme(Theme theme){
        base.ChangeTheme(theme);
        //update rayOrigin and rayLength for collision detection
        bc=activeSprite.GetComponent<Collider2D>();
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
        activeSprite.SetActive(false);
        yield return new WaitForSeconds(disappearDuration);
        activeSprite.SetActive(true);
        trapActivated=false;
    }
}
