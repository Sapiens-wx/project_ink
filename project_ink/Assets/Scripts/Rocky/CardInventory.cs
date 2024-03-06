using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "card inventory", menuName = "Inventory/card inventory")]
public class CardInventory : ScriptableObject
{
    public List<Card> cards;
}
