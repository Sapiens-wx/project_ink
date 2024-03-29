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
    public IEnumerator SetCard_Anim(Card _card)
    {
        card = _card;
        if (_card == null)
        {
            float interval = .4f;
            int cnt = (int)(interval / Time.fixedDeltaTime);
            WaitForFixedUpdate wffu = new WaitForFixedUpdate();
            image.transform.localScale = Vector3.one;
            for(int i = 0; i < cnt; ++i)
            {
                image.transform.localScale = Vector3.Lerp(image.transform.localScale, Vector3.zero, .2f);
                yield return wffu;
            }
            image.gameObject.SetActive(false);
        }
        else
        {
            float interval = 1;
            int cnt = (int)(interval / Time.fixedDeltaTime);
            WaitForFixedUpdate wffu = new WaitForFixedUpdate();
            image.transform.localScale = Vector3.zero;
            image.sprite = card.image;
            image.gameObject.SetActive(true);
            for(int i = 0; i < cnt; ++i)
            {
                image.transform.localScale = Vector3.Lerp(image.transform.localScale, Vector3.one, .1f);
                yield return wffu;
            }
            image.transform.localScale = Vector3.one;
        }
    }
}
