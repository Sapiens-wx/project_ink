using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_8", menuName = "Inventory/Cards/Tentacle/8")]
public class Card_T_8 : Card_T_Base
{
    public override Card Copy(){
        Card_T_8 ret=ScriptableObject.CreateInstance<Card_T_8>();;
        CopyTo(ret);
        return ret;
    }
}
