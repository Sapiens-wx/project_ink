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
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        if(damage>0) actions.Add(Delay(CalcRecoverTime(1)));
        if(TentacleManager.inst.BookCount<3){
            actions.Add(IEnumAction(()=>TentacleManager.inst.AddNTentacles(3)));
        } else{
            actions.Add(IEnumAction(()=>TentacleManager.inst.Pray(3)));
        }
    }
}
