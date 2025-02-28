using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_N_Discard", menuName = "Inventory/Cards/Normal/Discard")]
public class Card_N_Discard : Card
{
    public override Card Copy()
    {
        Card_N_Discard ret = ScriptableObject.CreateInstance<Card_N_Discard>();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        int i = slotIndex + 1;
        if(i<CardSlotManager.inst.numSlots && CardSlotManager.inst.cardSlots[i].card!=null)
            CardSlotManager.inst.cardSlots[i].card.Prep_Discard(actions);
    }
}