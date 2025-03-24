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
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        //auto fire as many top card in card pile as the number of tentacles the player has
        if(TentacleManager.inst.Rank>1){
            for(int i=TentacleManager.inst.BookCount;i>0;--i){
                Card top=CardSlotManager.inst.cardDealer.GetCard();
                if(top!=null){
                    //auto fire the card
                    actions.Add(top.AutoFire(false));
                }
            }
            actions.Add(IEnumAction(()=>TentacleManager.inst.Reconcile(3)));
        }
    }
}
