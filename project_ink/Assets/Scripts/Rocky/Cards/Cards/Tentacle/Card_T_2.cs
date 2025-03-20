using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_2", menuName = "Inventory/Cards/Tentacle/2")]
public class Card_T_2 : Card_T_Base
{
    public override Card Copy(){
        Card_T_2 ret=ScriptableObject.CreateInstance<Card_T_2>();;
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        //consume the next card, and pray
        if(SlotIndex!=-1 && SlotIndex<CardSlotManager.inst.cardSlots.Length-1){
            Card nextCard=CardSlotManager.inst.cardSlots[SlotIndex+1].card;
            if(nextCard!=null) nextCard.Prep_Consume(actions);
            actions.Add(IEnumAction(()=>{
                TentacleManager.inst.Pray(1);
            }));
        }
    }
}
