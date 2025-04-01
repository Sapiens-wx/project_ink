using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_Pig_Attack1 : StateBase<E_Pig>
{
    float jumpX;
    Vector3[] oriScalePig, oriPosPig;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        oriScalePig=new Vector3[3];
        oriPosPig=new Vector3[3];
        oriScalePig[0]=ctrller.pig[0].transform.localScale;
        oriScalePig[1]=ctrller.pig[1].transform.localScale;
        oriScalePig[2]=ctrller.pig[2].transform.localScale;
        oriPosPig[0]=ctrller.pig[0].transform.position;
        oriPosPig[1]=ctrller.pig[1].transform.position;
        oriPosPig[2]=ctrller.pig[2].transform.position;
        ctrller.animators[0].SetBool("idle",true);
        ctrller.animators[1].SetBool("idle",true);
        ctrller.animators[2].SetBool("idle",true);
        ctrller.StartCoroutine(Jump());
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ctrller.animators[0].SetBool("idle",false);
        ctrller.animators[1].SetBool("idle",false);
        ctrller.animators[2].SetBool("idle",false);
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
    IEnumerator Jump(int idx, float xDist, bool startAnim, bool endAnim){
        float squeezey=oriScalePig[idx].y*ctrller.animScaleYMin;
        float restorey=oriScalePig[idx].y;
        float squeezePosY=oriPosPig[idx].y+(ctrller.animScaleYMin-1)/2*ctrller.pig[idx].bounds.size.y;
        float restorePosY=oriPosPig[idx].y;
        float squeezex=oriScalePig[idx].x*ctrller.animScaleXMax;
        float restorex=oriScalePig[idx].x;
        float stretchX=oriScalePig[idx].x*ctrller.animStretchX;
        float stretchY=oriScalePig[idx].y*ctrller.animStretchY;
        float jumpIntervalHalf=ctrller.jumpInterval/2;

        if(startAnim){
            //Squeeze
            ctrller.pig[idx].transform.DOScaleY(squeezey, ctrller.animInterval);
            ctrller.pig[idx].transform.DOScaleX(squeezex, ctrller.animInterval);
            ctrller.pig[idx].transform.DOMoveY(squeezePosY, ctrller.animInterval);
            yield return new WaitForSeconds(ctrller.animInterval);
        }

        //jump horizontal movement
        ctrller.transform.DOMoveX(ctrller.pig[idx].transform.position.x+xDist, ctrller.jumpInterval);
        //first half of the jumping (move y to the top)
        ctrller.pig[idx].transform.DOMoveY(restorePosY+ctrller.jumpHeight, jumpIntervalHalf).SetEase(Ease.OutQuad);
        //stretch Y, until reach the top during the jump
        ctrller.pig[idx].transform.DOScaleY(stretchY, jumpIntervalHalf);
        ctrller.pig[idx].transform.DOScaleX(stretchX, jumpIntervalHalf);
        yield return new WaitForSeconds(jumpIntervalHalf);
        //remaining half of the jumping
        ctrller.pig[idx].transform.DOMoveY(restorePosY, jumpIntervalHalf).SetEase(Ease.InQuad);
        //restore scale to original scale
        ctrller.pig[idx].transform.DOScale(new Vector3(restorex, restorey, 1), jumpIntervalHalf);
        yield return new WaitForSeconds(jumpIntervalHalf);

        //squeeze
        ctrller.pig[idx].transform.DOScaleY(squeezey, ctrller.animInterval);
        ctrller.pig[idx].transform.DOScaleX(squeezex, ctrller.animInterval);
        ctrller.pig[idx].transform.DOMoveY(squeezePosY, ctrller.animInterval);
        yield return new WaitForSeconds(ctrller.animInterval);

        if(endAnim){
            //restore
            ctrller.pig[idx].transform.DOScaleY(restorey, ctrller.animInterval);
            ctrller.pig[idx].transform.DOScaleX(restorex, ctrller.animInterval);
            ctrller.pig[idx].transform.DOMoveY(restorePosY, ctrller.animInterval);
            yield return new WaitForSeconds(ctrller.animInterval);
        }
    }
    IEnumerator Jump(){
        IEnumerator ienum=Jump(0, JumpDist(), true, false);
        while(ienum.MoveNext())
            yield return ienum.Current;
        for(int i=0;i<3;++i){
            ienum=Jump(0, JumpDist(), false, false);
            while(ienum.MoveNext())
                yield return ienum.Current;
        }
        ienum=Jump(0, JumpDist(), false, true);
        while(ienum.MoveNext())
            yield return ienum.Current;
        ctrller.animator.SetTrigger("idle");
        yield break;
    }
}