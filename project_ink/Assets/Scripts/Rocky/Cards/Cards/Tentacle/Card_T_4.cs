using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_4", menuName = "Inventory/Cards/Tentacle/4")]
public class Card_T_4 : Card_T_Base
{
    public override Card Copy(){
        Card_T_4 ret=ScriptableObject.CreateInstance<Card_T_4>();;
        CopyTo(ret);
        return ret;
    }
}
