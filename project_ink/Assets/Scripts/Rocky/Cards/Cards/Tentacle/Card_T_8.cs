using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_8", menuName = "Inventory/Cards/Tentacle/8")]
public class Card_T_8 : Card_T_Base
{
    public override Card Copy(){
        Card_T_8 ret=ScriptableObject.CreateInstance<Card_T_8>();;
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Consume(List<IEnumerator> actions)
    {
        //auto activate four times
        actions.Add(Activate(false));
        actions.Add(Activate(false));
        actions.Add(Activate(false));
        actions.Add(Activate(false));
        actions.Add(Delay(CalcRecoverTime(4))); //recover time
        //return to card pool
        actions.Add(IEnumAction(ReturnToCardPool));
    }
}
