using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_2", menuName = "Inventory/Cards/1/2")]
public class Card_1_2 : Card
{
    public override Card Copy()
    {
        Card_1_2 ret = new Card_1_2();
        CopyTo(ret);
        return ret;
    }
    public override IEnumerator OnShot(CardSlot slot, System.Action callback)
    {
        int n = Mathf.Min(CardSlotManager.instance.numSlots, slotIndex + 3);
        for(int i = slotIndex + 1; i < n; ++i)
        {
            yield return new WaitForSeconds(0.3f);
            CardSlotManager.instance.DiscardCardInSlot(i);
        }
        callback?.Invoke();
        ReturnToCardPool();
        yield break;
    }
}
