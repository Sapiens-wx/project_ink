using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_P_1", menuName = "Inventory/Cards/Planet/1")]
public class Card_P_1 : Card
{
    public override Card Copy()
    {
        Card_1_3 ret = ScriptableObject.CreateInstance<Card_1_3>();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        actions.Add(Effect());
        int n = Mathf.Min(CardSlotManager.inst.numSlots, slotIndex + 3);
        for(int i = slotIndex + 1; i < n; ++i)
        {
            if(CardSlotManager.inst.cardSlots[i].card!=null)
                CardSlotManager.inst.cardSlots[i].card.Prep_Discard(actions);
        }
    }
    IEnumerator Effect(){
        CardSlotManager.inst.buff1_3.Enable(3, .5f);
        yield break;
    }
}
