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
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        int i=TentacleManager.inst.BookCount;
        //activate this card n times. n=book count
        for(;i>1;--i){
            actions.Add(Activate(false));
        }
        actions.Add(Delay(CalcRecoverTime(TentacleManager.inst.BookCount))); //recover time
        //if hit an enemy, remove a tentacle and increase the damage of every tentacle by 1
        TentacleManager.inst.tentacle.onHitEnemy+=OnHitEnemy;
    }
    //if hit an enemy, remove a tentacle and increase the damage of every tentacle by 1
    void OnHitEnemy(){
        TentacleManager.inst.RemoveATentacle();
        for(int i=TentacleManager.inst.BookCount-1;i>-1;--i){
            TentacleManager.inst.books[i].accumulatedDamage++;
        }
        TentacleManager.inst.tentacle.onHitEnemy-=OnHitEnemy;
    }
}
