using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_3", menuName = "Inventory/Cards/1/3")]
public class Card_1_3 : Card
{
    public override Card Copy()
    {
        Card_1_3 ret = new Card_1_3();
        CopyTo(ret);
        return ret;
    }
}
