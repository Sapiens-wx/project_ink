using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_Pig_Attack2 : StateBase<E_Pig>
{
    float squeezey, restorey, squeezePosY, squeezex, restorex;
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
        Bounds roomBoundsGlobal=RoomManager.CurrentRoom.RoomGlobalBounds;
        float leftDest=roomBoundsGlobal.min.x+ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
        float rightDest=roomBoundsGlobal.max.x-ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
        bool lastDestIsLeft;
        ctrller.UpdateDir();
        if(ctrller.Dir==1){
            lastDestIsLeft=false;
            yield return new WaitForSeconds(Jump(ctrller.pig1, oriScalePig1, oriPosPig1, rightDest-ctrller.transform.position.x, true, false));
        } else{
            lastDestIsLeft=true;
            yield return new WaitForSeconds(Jump(ctrller.pig1, oriScalePig2, oriPosPig2, leftDest-ctrller.transform.position.x, true, false));
        }
        yield return new WaitForSeconds(Jump(ctrller.pig2, oriScalePig2, oriPosPig2, (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x, false, false));
        lastDestIsLeft=!lastDestIsLeft;
        yield return new WaitForSeconds(Jump(ctrller.pig3, oriScalePig2, oriPosPig2, (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x, false, true));
        ctrller.animator.SetTrigger("idle");
    }
}