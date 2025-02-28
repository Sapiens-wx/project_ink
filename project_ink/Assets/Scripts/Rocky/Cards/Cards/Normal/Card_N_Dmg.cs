using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_N_Dmg", menuName = "Inventory/Cards/Normal/Damage")]
public class Card_N_Dmg : Card
{
    public override Card Copy()
    {
        Card_N_Dmg ret = ScriptableObject.CreateInstance<Card_N_Dmg>();
        CopyTo(ret);
        return ret;
    }
}
