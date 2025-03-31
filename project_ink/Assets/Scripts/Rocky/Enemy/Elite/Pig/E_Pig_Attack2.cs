using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_Pig_Attack2 : StateBase<E_Pig>
{
    Vector3[] oriScalePig, oriPosPig;
    AnimParams[] animParams;
    float prevx, maxDeltaX; //mexDeltaX
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        UpdateAnimParams();
        ctrller.StartCoroutine(Jump());
        prevx=ctrller.transform.position.x;
        maxDeltaX=RoomManager.CurrentRoom.RoomBounds.size.x/ctrller.ac2_jumpInterval*Time.fixedDeltaTime;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        SetPigAnimatorsSpd(Mathf.Clamp01(Mathf.Abs(ctrller.transform.position.x-prevx)/maxDeltaX));
        prevx=ctrller.transform.position.x;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        for(int i=0;i<3;++i){
            ctrller.pig[i].transform.localPosition=oriPosPig[i];
        }
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
            yield return new WaitForSeconds(Jump(jumps[0], jumps[1], rightDest-ctrller.transform.position.x, true, false));
        } else{
            lastDestIsLeft=true;
            yield return new WaitForSeconds(Jump(jumps[0], jumps[1], leftDest-ctrller.transform.position.x, true, false));
        }
        //second jump
        SetAnimatorParams(false,jumps[1]<1,jumps[1]<2);
        yield return new WaitForSeconds(Jump(jumps[1], jumps[2], (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x, false, false));
        //third jump
        lastDestIsLeft=!lastDestIsLeft;
        SetAnimatorParams(false,jumps[2]<1,jumps[2]<2);
        yield return new WaitForSeconds(Jump(jumps[2], jumps[2], (lastDestIsLeft?rightDest:leftDest)-ctrller.transform.position.x, false, true));
        ctrller.animator.SetTrigger("idle");
    }
    class AnimParams{
        public float squeezey, restorey, squeezePosY, restorePosY, squeezeX, restoreX;
    }
    void UpdateAnimParams(){
        oriPosPig=new Vector3[3];
        oriScalePig=new Vector3[3];
        animParams=new AnimParams[3];
        oriScalePig[0]=ctrller.pig[0].transform.localScale;
        oriScalePig[1]=ctrller.pig[1].transform.localScale;
        oriScalePig[2]=ctrller.pig[2].transform.localScale;
        oriPosPig[0]=ctrller.pig[0].transform.localPosition;
        oriPosPig[1]=ctrller.pig[1].transform.localPosition;
        oriPosPig[2]=ctrller.pig[2].transform.localPosition;
        for(int i=0;i<3;++i){
            animParams[i]=new AnimParams();
            animParams[i].squeezey=oriScalePig[i].y*ctrller.animScaleYMin;
            animParams[i].restorey=oriScalePig[i].y;
            animParams[i].squeezePosY=oriPosPig[i].y+(ctrller.animScaleYMin-1)/2*ctrller.pig[i].bounds.size.y;
            animParams[i].restorePosY=oriPosPig[i].y;
            animParams[i].squeezeX=oriScalePig[i].x*ctrller.animScaleXMax;
            animParams[i].restoreX=oriScalePig[i].x;
        }
    }
    float Jump(int idx, int nextIdx, float xDist, bool startAnim, bool endAnim){
        ++idx;
        ++nextIdx;
        float time=0;
        if(idx>2){ //only roll
            Sequence _s=DOTween.Sequence();
            _s.Join(ctrller.transform.DOMoveX(ctrller.transform.position.x+xDist, ctrller.ac2_jumpInterval));
            return ctrller.ac2_jumpInterval;
        }

        Sequence s=DOTween.Sequence();
        if(startAnim){
            time+=ctrller.animInterval;
            //Squeeze
            s.Append(ctrller.pig[idx].transform.DOScaleY(animParams[idx].squeezey, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOScaleX(animParams[idx].squeezeX, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOLocalMoveY(animParams[idx].squeezePosY, ctrller.animInterval));
        }

        time+=ctrller.ac2_jumpInterval;
        //restore
        s.Append(ctrller.pig[idx].transform.DOScaleY(animParams[idx].restorey, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOScaleX(animParams[idx].restoreX, ctrller.animInterval));
        s.Join(ctrller.pig[idx].transform.DOLocalMoveY(animParams[idx].restorePosY, ctrller.animInterval));
        //jump
        s.Join(ctrller.transform.DOMoveX(ctrller.pig[idx].transform.position.x+xDist, ctrller.ac2_jumpInterval));
        s.Join(ctrller.pig[idx].transform.DOMoveY(ctrller.pig[idx].transform.position.y+ctrller.jumpHeight, ctrller.jumpInterval/2).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad));

        time+=ctrller.animInterval;
        //squeeze
        if(nextIdx>=0&&nextIdx<3&&nextIdx!=idx){
            s.AppendCallback(()=>{ctrller.pig[idx].transform.localPosition=oriPosPig[idx];}); //if don't add this, the pig will not return to its original position
            s.Append(ctrller.pig[nextIdx].transform.DOScaleY(animParams[nextIdx].squeezey, ctrller.animInterval));
            s.Join(ctrller.pig[nextIdx].transform.DOScaleX(animParams[nextIdx].squeezeX, ctrller.animInterval));
            s.Join(ctrller.pig[nextIdx].transform.DOLocalMoveY(animParams[nextIdx].squeezePosY, ctrller.animInterval));
        } else if(nextIdx<3||endAnim){
            s.Append(ctrller.pig[idx].transform.DOScaleY(animParams[idx].squeezey, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOScaleX(animParams[idx].squeezeX, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOLocalMoveY(animParams[idx].squeezePosY, ctrller.animInterval));
        }

        if(endAnim){
            time+=ctrller.animInterval;
            //restore
            s.Append(ctrller.pig[idx].transform.DOScaleY(animParams[idx].restorey, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOScaleX(animParams[idx].restoreX, ctrller.animInterval));
            s.Join(ctrller.pig[idx].transform.DOLocalMoveY(animParams[idx].restorePosY, ctrller.animInterval));
        }
        return time;
    }
}