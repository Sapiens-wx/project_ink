using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Card card;
    public Image image;
    [HideInInspector] public int index;
    
    public void SetCard(Card _card)
    {
        card = _card;
        image.sprite = card.image;
    }
}
