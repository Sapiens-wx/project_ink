using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CardBagInfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public Button button;
    public TMP_Text cardName, cardCount;
    [HideInInspector] public CardInventory.CardInfo cardInfo;

    void Start(){
        button.onClick.AddListener(()=>{CardSelectManager.inst.FromBagToInv(this);});
    }
    public void UpdateInfo(CardInventory.CardInfo cardInfo){
        cardName.text=cardInfo.card.name;
        cardCount.text=cardInfo.count.ToString();
        this.cardInfo=cardInfo;
    }
    public void OnPointerEnter(PointerEventData eventData){
        CardSelectManager.inst.cardTips.ShowTip(cardInfo.card);
        CardZoomInTip.ShowTip(cardInfo.card);
    }
    public void OnPointerExit(PointerEventData eventData){
        CardZoomInTip.HideTip();
    }
}