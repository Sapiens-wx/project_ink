using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;
using Unity.VisualScripting;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_Pig_Attack2 : StateBase<E_Pig>
{
    Vector3[] oriScalePig, oriPosPig;
    AnimParams[] animParams;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        UpdateAnimParams();
        ctrller.StartCoroutine(Jump());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        SetAnimatorParams(true,true,true);
        SetPigAnimatorsSpd(1);
    }
    void SetPigAnimatorsSpd(float spd){
        ctrller.animators[0].speed=spd;
        ctrller.animators[1].speed=spd;
        ctrller.animators[2].speed=spd;
    }
    /// <summary>
    /// Generates a random sequence of 0,1,2
    /// </summary>
    int[] GetJump(){
        int[] res=new int[3];
        res[0]=Random.Range(0,3);
        switch(res[0]){
            case 0:
                res[1]=Random.Range(1,3);
                break;
            case 1:
                res[1]=Random.Range(0,2)==0?0:2;
                break;
            case 2:
                res[1]=Random.Range(0,2);
                break;
        }
        res[2]=3-res[1]-res[0];
        return res;
    }
    /// <summary>
    /// sets the three pigs' animators' parameters
    /// </summary>
    void SetAnimatorParams(bool b1, bool b2, bool b3){
        ctrller.animators[0].SetBool("idle",b1);
        ctrller.animators[1].SetBool("idle",b2);
        ctrller.animators[2].SetBool("idle",b3);
    }
    IEnumerator Jump(){
        IEnumerator ienum;
        Bounds roomBoundsGlobal=RoomManager.CurrentRoom.RoomBounds;
        float leftDest=roomBoundsGlobal.min.x+ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
        float rightDest=roomBoundsGlobal.max.x-ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
        bool lastDestIsLeft;
        ctrller.UpdateDir();
        int[] jumps=GetJump(); //0 means pig 0 roll. 1 means pig 0&1 roll.
        //first jump
        SetAnimatorParams(false,jumps[0]<1,jumps[0]<2);
        if(ctrller.Dir==1){
            lastDestIsLeft=false;
            ienum=Jump(jumps[0], rightDest-ctrller.transform.position.x);
            while(ienum.MoveNext())
                yield return ienum.Current;
        } else{
            lastDestIsLeft=true;
            ienum=Jump(jumps[0], leftDest-ctrller.transform.position.x);
            while(ienum.MoveNext())
                yield return ienum.Current;
        }
        //second jump
        ctrller.Dir=ctrller.Dir==1?-1:1;
        SetAnimatorParams(false,jumps[1]<1,jumps[1]<2);
        ienum=Jump(jumps[1], (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x);
        while(ienum.MoveNext())
            yield return ienum.Current;
        //third jump
        ctrller.Dir=ctrller.Dir==1?-1:1;
        lastDestIsLeft=!lastDestIsLeft;
        SetAnimatorParams(false,jumps[2]<1,jumps[2]<2);
        ienum=Jump(jumps[2], (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x);
        while(ienum.MoveNext())
            yield return ienum.Current;
        ctrller.animator.SetTrigger("idle");
    }
    class AnimParams{
        public float squeezeY, restoreY, squeezePosY, restorePosY, squeezeX, restoreX, stretchX, stretchY;
    }
    void UpdateAnimParams(){
        oriPosPig=new Vector3[3];
        oriScalePig=new Vector3[3];
        animParams=new AnimParams[3];
        oriScalePig[0]=ctrller.pig[0].transform.localScale;
        oriScalePig[1]=ctrller.pig[1].transform.localScale;
        oriScalePig[2]=ctrller.pig[2].transform.localScale;
        oriPosPig[0]=ctrller.pig[0].transform.position;
        oriPosPig[1]=ctrller.pig[1].transform.position;
        oriPosPig[2]=ctrller.pig[2].transform.position;
        for(int i=0;i<3;++i){
            animParams[i]=new AnimParams();
            animParams[i].squeezeY=oriScalePig[i].y*ctrller.animScaleYMin;
            animParams[i].restoreY=oriScalePig[i].y;
            animParams[i].squeezePosY=oriPosPig[i].y+(ctrller.animScaleYMin-1)/2*ctrller.pig[i].bounds.size.y;
            animParams[i].restorePosY=oriPosPig[i].y;
            animParams[i].squeezeX=oriScalePig[i].x*ctrller.animScaleXMax;
            animParams[i].restoreX=oriScalePig[i].x;
            animParams[i].stretchX=oriScalePig[i].x*ctrller.animStretchX;
            animParams[i].stretchY=oriScalePig[i].y*ctrller.animStretchY;
        }
    }
    void StartRoll(){
        float spd=0;
        DOTween.To(()=>spd,(t)=>SetPigAnimatorsSpd(t),1,ctrller.ac2_anticipation);
    }
    //duration=ctrller.animInterval
    IEnumerator EndRoll(int idx){
        AnimatorStateInfo info=ctrller.animators[idx].GetCurrentAnimatorStateInfo(0);
        float endTime=Time.time+ctrller.animInterval;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(Time.time<endTime){
            if(info.normalizedTime<0.05f || info.normalizedTime>0.95f){
                ctrller.animators[idx].speed=0;
            }
            yield return wait;
        }
        ctrller.animators[idx].speed=1;
    }
    IEnumerator Jump(int idx, float xDist){
        ++idx;
        IEnumerator endRollIenum=EndRoll(idx-1);
        float horizontalMoveInterval=Mathf.Abs(xDist)/ctrller.ac2_moveSpeed;
        if(idx>2){ //only roll
            StartRoll();
            yield return new WaitForSeconds(ctrller.ac2_anticipation);
            ctrller.transform.DOMoveX(ctrller.transform.position.x+xDist, horizontalMoveInterval);
            yield return new WaitForSeconds(horizontalMoveInterval);
            while(endRollIenum.MoveNext())
                yield return endRollIenum.Current;
            yield break;
        }
        float jumpIntervalHalf=horizontalMoveInterval/2;

        //Squeeze
        //has anticipation, so
        //|-----anticipation-----|
        //|---wait---|--squeeze--|
        StartRoll();
        float waitTime=ctrller.ac2_anticipation-ctrller.animInterval;
        yield return new WaitForSeconds(waitTime);
        ctrller.pig[idx].transform.DOScaleY(animParams[idx].squeezeY, ctrller.animInterval);
        ctrller.pig[idx].transform.DOScaleX(animParams[idx].squeezeX, ctrller.animInterval);
        ctrller.pig[idx].transform.DOMoveY(animParams[idx].squeezePosY, ctrller.animInterval);
        yield return new WaitForSeconds(ctrller.animInterval);

        //jump horizontal movement
        ctrller.transform.DOMoveX(ctrller.pig[idx].transform.position.x+xDist, horizontalMoveInterval).SetEase(Ease.InOutSine);
        //first half of the jumping (move y to the top)
        ctrller.pig[idx].transform.DOMoveY(animParams[idx].restorePosY+ctrller.jumpHeight, jumpIntervalHalf).SetEase(Ease.OutQuad);
        //stretch Y, until reach the top during the jump
        ctrller.pig[idx].transform.DOScaleY(animParams[idx].stretchY, jumpIntervalHalf);
        ctrller.pig[idx].transform.DOScaleX(animParams[idx].stretchX, jumpIntervalHalf);
        yield return new WaitForSeconds(jumpIntervalHalf);
        //remaining half of the jumping
        ctrller.pig[idx].transform.DOMoveY(animParams[idx].restorePosY, jumpIntervalHalf).SetEase(Ease.InQuad);
        //restore scale to original scale
        ctrller.pig[idx].transform.DOScale(new Vector3(animParams[idx].restoreX, animParams[idx].restoreY, 1), jumpIntervalHalf);
        yield return new WaitForSeconds(jumpIntervalHalf);

        //squeeze
        ctrller.pig[idx].transform.DOScaleY(animParams[idx].squeezeY, ctrller.animInterval);
        ctrller.pig[idx].transform.DOScaleX(animParams[idx].squeezeX, ctrller.animInterval);
        ctrller.pig[idx].transform.DOMoveY(animParams[idx].squeezePosY, ctrller.animInterval);
        yield return new WaitForSeconds(ctrller.animInterval);

        //restore
        ctrller.pig[idx].transform.DOScaleY(animParams[idx].restoreY, ctrller.animInterval);
        ctrller.pig[idx].transform.DOScaleX(animParams[idx].restoreX, ctrller.animInterval);
        ctrller.pig[idx].transform.DOMoveY(animParams[idx].restorePosY, ctrller.animInterval);
        while(endRollIenum.MoveNext())
            yield return endRollIenum.Current;
    }
}