using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_7", menuName = "Inventory/Cards/1/7")]
public class Card_1_7 : Card
{
    public override Card Copy()
    {
        Card_1_7 ret = new Card_1_7();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Discard(List<IEnumerator> actions)
    {
        base.Prep_Discard(actions);
        //implement "double damage"
        damage*=2;
    }
}
