using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_6", menuName = "Inventory/Cards/Tentacle/6")]
public class Card_T_6 : Card_T_Base
{
    public override Card Copy(){
        Card_T_6 ret=ScriptableObject.CreateInstance<Card_T_6>();;
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        //consume card in every cardslot
        for(int i=CardSlotManager.inst.cardSlots.Length-1;i>-1;--i){
            Card card=CardSlotManager.inst.cardSlots[i].card;
            if(card!=null) card.Prep_Consume(actions);
        }
        TentacleManager.inst.AddNTentacles(5);
        TentacleManager.inst.Pray(3);
        TentacleManager.inst.canReconcile=true;
    }
}
