using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_7", menuName = "Inventory/Cards/Tentacle/7")]
public class Card_T_7 : Card_T_Base
{
    public override Card Copy(){
        Card_T_7 ret=ScriptableObject.CreateInstance<Card_T_7>();;
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        //deal 1 damage three times
        actions.Add(Activate(false));
        actions.Add(Activate(false));
        actions.Add(Delay(CalcRecoverTime(3))); //recover time
        //consume the next two cards
        for(int i=Mathf.Min(SlotIndex+2,CardSlotManager.inst.cardSlots.Length-1);i>SlotIndex;--i){
            Card card=CardSlotManager.inst.cardSlots[i].card;
            if(card!=null) card.Prep_Consume(actions);
        }
        //add two more cards to card dealer
        actions.Add(IEnumAction(()=>{
            CardSlotManager.inst.cardDealer.ReturnToCardPool(Copy());
            CardSlotManager.inst.cardDealer.ReturnToCardPool(Copy());
            }));
    }
    public override void Prep_Consume(List<IEnumerator> actions)
    {
        base.Prep_Consume(actions);
        actions.Add(IEnumAction(()=>{
            TentacleManager.inst.Pray(2);
            TentacleManager.inst.AddNTentacles(2);
        }));
    }
}
