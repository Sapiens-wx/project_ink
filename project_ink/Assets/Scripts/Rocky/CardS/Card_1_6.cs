using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_6", menuName = "Inventory/Cards/1/6")]
public class Card_1_6 : Card
{
    public override Card Copy()
    {
        Card_1_6 ret = new Card_1_6();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        int n = Mathf.Min(CardSlotManager.instance.numSlots, slotIndex + 4);
        for(int i = slotIndex + 1; i < n; ++i)
        {
            if(CardSlotManager.instance.cardSlots[i].card!=null)
                CardSlotManager.instance.cardSlots[i].card.Prep_Discard(actions);
        }
        for(int i=slotIndex+1;i<n;++i){
            actions.Add(CardSlotManager.instance.AssignCardToSlotRandomly_ienum(i));
        }
    }
}