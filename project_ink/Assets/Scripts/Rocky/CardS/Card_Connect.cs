using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "connect", menuName = "Inventory/Cards/Connect")]
public class Card_Connect : Card
{
    public override void OnShot(CardSlot slot)
    {
        if(slot.index<CardSlotManager.instance.numSlots-1)
        {
            CardSlotManager.instance.Shoot();
        }
    }
}
