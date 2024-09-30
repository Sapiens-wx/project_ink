using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_2", menuName = "Inventory/Cards/1/2")]
public class Card_1_2 : Card
{
    public override Card Copy()
    {
        Card_1_2 ret = ScriptableObject.CreateInstance<Card_1_2>();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        int n = Mathf.Min(CardSlotManager.instance.numSlots, slotIndex + 3);
        for(int i = slotIndex + 1; i < n; ++i)
        {
            if(CardSlotManager.instance.cardSlots[i].card!=null){
                CardSlotManager.instance.cardSlots[i].card.Prep_Discard(actions);
            }
        }
    }
}
