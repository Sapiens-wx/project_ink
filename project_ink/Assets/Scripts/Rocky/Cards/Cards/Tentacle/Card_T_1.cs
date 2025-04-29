using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_T_1", menuName = "Inventory/Cards/Tentacle/1")]
public class Card_T_1 : Card_T_Base
{
    public override Card Copy(){
        Card_T_1 ret=ScriptableObject.CreateInstance<Card_T_1>();;
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        TentacleManager.inst.tentacle.onHitEnemy+=HitEnemyEffect;
        base.Prep_Fire(actions);
        actions.Add(Delay(CalcRecoverTime(1))); //recover time
    }
    //if hit, add a tentacle
    void HitEnemyEffect(EnemyBase enemy){
        TentacleManager.inst.AddNTentacles(1);
        TentacleManager.inst.tentacle.onHitEnemy-=HitEnemyEffect;
    }
}
