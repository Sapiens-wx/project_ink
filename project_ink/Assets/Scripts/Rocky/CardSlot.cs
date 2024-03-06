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
        if (_card == null)
            image.gameObject.SetActive(false);
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = card.image;
        }
    }
}
