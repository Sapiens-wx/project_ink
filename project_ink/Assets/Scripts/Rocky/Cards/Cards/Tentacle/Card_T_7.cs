using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_7", menuName = "Inventory/Cards/Tentacle/7")]
public class Card_T_7 : Card_T_Base
{
    public override Card Copy(){
        Card_T_7 ret=ScriptableObject.CreateInstance<Card_T_7>();;
        CopyTo(ret);
        return ret;
    }
}
