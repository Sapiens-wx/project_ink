using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_4", menuName = "Inventory/Cards/1/4")]
public class Card_1_4 : Card
{
    public override Card Copy()
    {
        Card_1_4 ret = ScriptableObject.CreateInstance<Card_1_4>();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Discard(List<IEnumerator> actions)
    {
        base.Prep_Discard(actions);
        CardSlotManager.inst.buff1_4.Enable();
    }
}
