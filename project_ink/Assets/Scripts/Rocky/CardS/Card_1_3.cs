using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_3", menuName = "Inventory/Cards/1/3")]
public class Card_1_3 : Card
{
    public override Card Copy()
    {
        Card_1_3 ret = new Card_1_3();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        //reduce anticipation time
        CardSlotManager.instance.anticReduceAmount=0.5f;
        CardSlotManager.instance.anticReduceCount=3;

        base.Prep_Fire(actions);
        int n = Mathf.Min(CardSlotManager.instance.numSlots, slotIndex + 3);
        for(int i = slotIndex + 1; i < n; ++i)
        {
            if(CardSlotManager.instance.cardSlots[i].card!=null)
                CardSlotManager.instance.cardSlots[i].card.Prep_Discard(actions);
        }
    }
}
