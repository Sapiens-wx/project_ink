using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_1", menuName = "Inventory/Cards/1/1")]
public class Card_1_1 : Card
{
    public override Card Copy()
    {
        Card_1_1 ret = new Card_1_1();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Discard(List<IEnumerator> actions)
    {
        actions.Add(AutoFire());
        base.Prep_Discard(actions);
        actions.Add(CardSlotManager.instance.AssignCardToSlotRandomly_ienum(slotIndex));
    }
}
