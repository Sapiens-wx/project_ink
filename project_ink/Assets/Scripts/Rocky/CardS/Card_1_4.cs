using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_4", menuName = "Inventory/Cards/1/4")]
public class Card_1_4 : Card
{
    public override Card Copy()
    {
        Card_1_4 ret = new Card_1_4();
        CopyTo(ret);
        return ret;
    }
}
