using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_Pig_Attack1 : StateBase<E_Pig>
{
    float jumpX;
    Vector3 oriScalePig1,oriScalePig2,oriScalePig3; //used by E_Pig_Attack1/2
    Vector3 oriPosPig1,oriPosPig2,oriPosPig3; //used by E_Pig_Attack1/2
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        oriScalePig1=ctrller.pig1.transform.localScale;
        oriScalePig2=ctrller.pig2.transform.localScale;
        oriScalePig3=ctrller.pig3.transform.localScale;
        oriPosPig1=ctrller.pig1.transform.position;
        oriPosPig2=ctrller.pig2.transform.position;
        oriPosPig3=ctrller.pig3.transform.position;
        ctrller.StartCoroutine(Jump());
    }
    Vector2 JumpVelocity(){
        ctrller.UpdateDir();
        float dist=Mathf.Abs(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x);
        dist=Mathf.Clamp(dist, ctrller.jumpXMin, ctrller.jumpXMax);
        jumpX=ctrller.Dir==1?dist:-dist;
        Vector2 v=Vector2.zero;
        v.x=jumpX/ctrller.jumpInterval;
        v.y=0.5f*ctrller.jumpInterval*ctrller.rgb.gravityScale*9.8f;
        return v;
    }
    float JumpDist(){
        ctrller.UpdateDir();
        float dist=Mathf.Abs(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x);
        dist=Mathf.Clamp(dist, ctrller.jumpXMin, ctrller.jumpXMax);
        dist=ctrller.Dir==1?dist:-dist;
        return dist;
    }
    float Jump(Collider2D target, Vector3 oriScale, Vector3 oriPos, float xDist, bool startAnim, bool endAnim){
        float time=0;

        float squeezey=oriScale.y*ctrller.animScaleYMin;
        float restorey=oriScale.y;
        float squeezePosY=oriPos.y+(ctrller.animScaleYMin-1)/2*target.bounds.size.y;
        float restorePosY=oriPos.y;
        float squeezex=oriScale.x*ctrller.animScaleXMax;
        float restorex=oriScale.x;

        Sequence s=DOTween.Sequence();
        if(startAnim){
            time+=ctrller.animInterval;
            //Squeeze
            s.Append(target.transform.DOScaleY(squeezey, ctrller.animInterval));
            s.Join(target.transform.DOScaleX(squeezex, ctrller.animInterval));
            s.Join(target.transform.DOMoveY(squeezePosY, ctrller.animInterval));
        }

        time+=ctrller.jumpInterval;
        //restore
        s.Append(target.transform.DOScaleY(restorey, ctrller.animInterval));
        s.Join(target.transform.DOScaleX(restorex, ctrller.animInterval));
        s.Join(target.transform.DOMoveY(restorePosY, ctrller.animInterval));
        //jump
        s.Join(ctrller.transform.DOMoveX(target.transform.position.x+xDist, ctrller.jumpInterval));
        s.Join(target.transform.DOMoveY(target.transform.position.y+ctrller.jumpHeight, ctrller.jumpInterval/2).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad));

        time+=ctrller.animInterval;
        //squeeze
        s.Append(target.transform.DOScaleY(squeezey, ctrller.animInterval));
        s.Join(target.transform.DOScaleX(squeezex, ctrller.animInterval));
        s.Join(target.transform.DOMoveY(squeezePosY, ctrller.animInterval));

        if(endAnim){
            time+=ctrller.animInterval;
            //restore
            s.Append(target.transform.DOScaleY(restorey, ctrller.animInterval));
            s.Join(target.transform.DOScaleX(restorex, ctrller.animInterval));
            s.Join(target.transform.DOMoveY(restorePosY, ctrller.animInterval));
        }
        return time;
    }
    IEnumerator Jump(){
        yield return new WaitForSeconds(Jump(ctrller.pig2, oriScalePig2, oriPosPig2, JumpDist(), true, false));
        for(int i=0;i<3;++i){
            yield return new WaitForSeconds(Jump(ctrller.pig2, oriScalePig2, oriPosPig2, JumpDist(), false, false));
        }
        yield return new WaitForSeconds(Jump(ctrller.pig2, oriScalePig2, oriPosPig2, JumpDist(), false, true));
        ctrller.animator.SetTrigger("idle");
        yield break;
        float squeezey=ctrller.transform.localScale.y*ctrller.animScaleYMin;
        float restorey=ctrller.transform.localScale.y;
        float squeezePosY=(ctrller.animScaleYMin-1)/2*ctrller.bc.bounds.size.y;
        float squeezex=ctrller.transform.localScale.x*ctrller.animScaleXMax;
        float restorex=ctrller.transform.localScale.x;
        //squeeze
        ctrller.spr.transform.DOScaleY(squeezey, ctrller.animInterval);
        ctrller.spr.transform.DOScaleX(squeezex, ctrller.animInterval);
        ctrller.spr.transform.DOLocalMoveY(squeezePosY, ctrller.animInterval);
        yield return new WaitForSeconds(ctrller.animInterval);
        for(int i=0;i<5;++i){
            //restore
            ctrller.spr.transform.DOScaleY(restorey, ctrller.animInterval);
            ctrller.spr.transform.DOScaleX(restorex, ctrller.animInterval);
            ctrller.spr.transform.DOLocalMoveY(0, ctrller.animInterval);
            Vector2 v=new Vector2((jumpX-ctrller.transform.position.x)/ctrller.jumpInterval, 0.5f*ctrller.jumpInterval*ctrller.rgb.gravityScale*9.8f);
            ctrller.rgb.velocity=JumpVelocity();
            yield return new WaitForSeconds(ctrller.jumpInterval);
            ctrller.rgb.velocity=Vector2.zero;
            //squeeze
            ctrller.spr.transform.DOScaleY(squeezey, ctrller.animInterval);
            ctrller.spr.transform.DOScaleX(squeezex, ctrller.animInterval);
            ctrller.spr.transform.DOLocalMoveY(squeezePosY, ctrller.animInterval);
            yield return new WaitForSeconds(ctrller.animInterval);
        }
        //restore
        ctrller.spr.transform.DOScaleY(restorey, ctrller.animInterval);
        ctrller.spr.transform.DOScaleX(restorex, ctrller.animInterval);
        ctrller.spr.transform.DOLocalMoveY(0, ctrller.animInterval);
        ctrller.animator.SetTrigger("idle");
    }
}