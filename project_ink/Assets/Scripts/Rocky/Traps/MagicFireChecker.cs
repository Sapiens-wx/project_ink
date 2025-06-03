using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this component is placed on the scene must have object
public class MagicFireChecker : Singleton<MagicFireChecker>
{
    ContactFilter2D filter;
    [HideInInspector] public bool isPlayerInsideFire;
    void Start()
    {
        filter=new ContactFilter2D();
        filter.useTriggers=true;
        filter.layerMask=GameManager.inst.playerLayer;
    }
    void FixedUpdate(){
        if(Trap_MagicFire.instances!=null){
            isPlayerInsideFire=false;
            foreach(Trap_MagicFire bc in Trap_MagicFire.instances){
                if(bc.bc.IsTouching(PlayerCtrl.inst.bc,filter)){
                    isPlayerInsideFire=true;
                    break;
                }
            }
        }
    }
}