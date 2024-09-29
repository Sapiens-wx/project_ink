using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_1_5", menuName = "Inventory/Cards/1/5")]
public class Card_1_5 : Card
{
    public override Card Copy()
    {
        Card_1_5 ret = new Card_1_5();
        CopyTo(ret);
        return ret;
    }
    public override void Prep_Fire(List<IEnumerator> actions)
    {
        base.Prep_Fire(actions);
        actions.Add(Effect());
    }
    IEnumerator Effect(){
        CardSlotManager.instance.StartCoroutine(ActualEffect());
        yield break;
    }
    IEnumerator ActualEffect(){
        CardSlotManager.instance.effect_card1_5++;
        yield return new WaitForSeconds(6);
        CardSlotManager.instance.effect_card1_5--;
    }
}