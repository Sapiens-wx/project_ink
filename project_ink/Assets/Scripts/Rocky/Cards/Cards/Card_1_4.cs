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
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        Consume();
        CardSlotManager.inst.buff1_4.Enable();
        base.Prep_Fire(actions);
    }
}
