using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_P_Uranus", menuName = "Inventory/Cards/Planet/Uranus")]
public class Card_P_Uranus : Card
{
    public override Card Copy()
    {
        Card_P_Uranus ret = ScriptableObject.CreateInstance<Card_P_Uranus>();
        CopyTo(ret);
        return ret;
    }
    public override void OnHitEnemy(EnemyBase enemy){
        PlanetVisualizer.inst.AddUranus(enemy, (Uranus)Planet.FromType(PlanetType.Uranus));
    }
}
