using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_P*", menuName = "Inventory/Cards/Planet/*")]
public class Card_P : Card
{
    public PlanetType planetType;
    public override Card Copy()
    {
        Card_P ret = ScriptableObject.CreateInstance<Card_P>();
        CopyTo(ret);
        ret.planetType=planetType;
        return ret;
    }
    public override void OnHitEnemy(EnemyBase enemy){
        if(planetType==PlanetType.Earth)
            CardSlotManager.inst.planetBuff.Earth();
        else
            PlanetManager.inst.AddPlanet(planetType);
    }
}
