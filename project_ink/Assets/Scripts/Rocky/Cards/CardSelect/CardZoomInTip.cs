using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardZoomInTip : Singleton<CardZoomInTip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text cardName, damage, description, explanation;
    [SerializeField] private Image img;

    Coroutine coro;
    void Start(){
        panel.SetActive(false);
    }
    IEnumerator ShowTipCoro(Card card){
        yield return new WaitForSeconds(1f);
        UpdateInfo(card);
        panel.SetActive(true);
        coro=null;
    }
    void UpdateInfo(Card card){
        cardName.text=card.type.ToString();
        damage.text=card.damage.ToString();
        description.text=card.description;
        explanation.text=card.explanation;
        img.sprite=card.image;
    }
    public static void ShowTip(Card card){
        if(inst.coro!=null)
            inst.StopCoroutine(inst.coro);
        inst.coro=inst.StartCoroutine(inst.ShowTipCoro(card));
    }
    public static void HideTip(){
        if(inst.coro!=null){
            inst.StopCoroutine(inst.coro);
            inst.coro=null;
        }
        inst.panel.SetActive(false);
    }
}
