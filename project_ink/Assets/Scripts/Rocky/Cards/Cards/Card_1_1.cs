using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_1", menuName = "Inventory/Cards/1/1")]
public class Card_1_1 : Card
{
    public override Card Copy()
    {
        Card_1_1 ret = ScriptableObject.CreateInstance<Card_1_1>();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Discard(List<IEnumerator> actions)
    {
        CardLog.DiscardCardEffect($"card 1: discarded->auto fire, randomly assign a card to the card slot");
        actions.Add(AutoFire(true));
        actions.Add(OnDiscardBuffCheck());
        actions.Add(CardSlotManager.inst.AssignCardToSlotRandomly_ienum(slotIndex));
    }
}
