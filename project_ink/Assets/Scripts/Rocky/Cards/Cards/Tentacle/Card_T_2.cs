using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_2", menuName = "Inventory/Cards/Tentacle/2")]
public class Card_T_2 : Card_T_Base
{
    public override Card Copy(){
        Card_T_2 ret=ScriptableObject.CreateInstance<Card_T_2>();;
        CopyTo(ret);
        return ret;
    }
}
