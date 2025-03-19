using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_5", menuName = "Inventory/Cards/Tentacle/5")]
public class Card_T_5 : Card_T_Base
{
    public override Card Copy(){
        Card_T_5 ret=ScriptableObject.CreateInstance<Card_T_5>();;
        CopyTo(ret);
        return ret;
    }
}
