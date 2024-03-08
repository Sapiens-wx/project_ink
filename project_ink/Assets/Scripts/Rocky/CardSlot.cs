using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Card card;
    public Image image;
    [HideInInspector] public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card != null)
        {
            image.transform.localScale = Vector3.one * 2;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (card != null)
        {
            image.transform.localScale = Vector3.one;
        }
    }

    public void SetCard(Card _card)
    {
        card = _card;
        if (_card == null)
        {
            image.transform.localScale = Vector3.one;
            image.gameObject.SetActive(false);
        }
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = card.image;
        }
    }
}
