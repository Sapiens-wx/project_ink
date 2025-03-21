using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEventManager
{
    /// <summary>
    /// triggered when a card deals damage to an enemy
    /// </summary>
    public static System.Action<HitEnemyInfo> onCardDealDamage;
}
