using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_3", menuName = "Inventory/Cards/Tentacle/3")]
public class Card_T_3 : Card_T_Base
{
    public override Card Copy(){
        Card_T_3 ret=ScriptableObject.CreateInstance<Card_T_3>();;
        CopyTo(ret);
        return ret;
    }
}
