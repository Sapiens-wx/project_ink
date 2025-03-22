using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEventManager
{
    /// <summary>
    /// triggered when a card deals damage to an enemy
    /// </summary>
    public static System.Action<HitEnemyInfo> onCardDealDamage;
    /// <summary>
    /// called when a new card cycle begins (card slot's current index reaches the end and set to 0, so any empty card slot is dealt with a card)
    /// </summary>
    public static System.Action onDistributeCard;
}
