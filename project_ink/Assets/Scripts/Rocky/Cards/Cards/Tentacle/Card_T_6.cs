using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_6", menuName = "Inventory/Cards/Tentacle/6")]
public class Card_T_6 : Card_T_Base
{
    public override Card Copy(){
        Card_T_6 ret=ScriptableObject.CreateInstance<Card_T_6>();;
        CopyTo(ret);
        return ret;
    }
}
