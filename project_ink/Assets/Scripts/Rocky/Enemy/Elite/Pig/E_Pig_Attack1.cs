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
    float Jump(int idx, float xDist, bool startAnim, bool endAnim){
        float time=0;

        float squeezey=oriScalePig[idx].y*ctrller.animScaleYMin;
        float restorey=oriScalePig[idx].y;
        float squeezePosY=oriPosPig[idx].y+(ctrller.animScaleYMin-1)/2*ctrller.pig[idx].bounds.size.y;
        float restorePosY=oriPosPig[idx].y;
        float squeezex=oriScalePig[idx].x*ctrller.animScaleXMax;
        float restorex=oriScalePig[idx].x;

        Sequence s=DOTween.Sequence();
        if(startAnim){
            time+=ctrller.animInterval;
            //Squeeze
            s.Append(ctrller.pig[idx].transform.DOScaleY(squeezey, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOScaleX(squeezex, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOMoveY(squeezePosY, ctrller.animInterval));
        }

        time+=ctrller.jumpInterval;
        //restore
        s.Append(ctrller.pig[idx].transform.DOScaleY(restorey, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOScaleX(restorex, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOMoveY(restorePosY, ctrller.animInterval));
        //jump
        s.Join(ctrller.transform.DOMoveX(ctrller.pig[idx].transform.position.x+xDist, ctrller.jumpInterval));
        s.Join(ctrller.pig[idx].transform.DOMoveY(ctrller.pig[idx].transform.position.y+ctrller.jumpHeight, ctrller.jumpInterval/2).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad));

        time+=ctrller.animInterval;
        //squeeze
        s.Append(ctrller.pig[idx].transform.DOScaleY(squeezey, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOScaleX(squeezex, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOMoveY(squeezePosY, ctrller.animInterval));

        if(endAnim){
            time+=ctrller.animInterval;
            //restore
            s.Append(ctrller.pig[idx].transform.DOScaleY(restorey, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOScaleX(restorex, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOMoveY(restorePosY, ctrller.animInterval));
        }
        return time;
    }
    IEnumerator Jump(){
        yield return new WaitForSeconds(Jump(0, JumpDist(), true, false));
        for(int i=0;i<3;++i){
            yield return new WaitForSeconds(Jump(0, JumpDist(), false, false));
        }
        yield return new WaitForSeconds(Jump(0, JumpDist(), false, true));
        ctrller.animator.SetTrigger("idle");
        yield break;
    }
}