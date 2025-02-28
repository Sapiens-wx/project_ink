using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardInvInfoUI : MonoBehaviour, IPointerEnterHandler{
    public Button button;
    public TMP_Text cardName, cardCount;
    public Image cardImg;
    [HideInInspector] public CardInventory.CardInfo cardInfo;
    void Start(){
        button.onClick.AddListener(()=>{CardSelectManager.inst.FromInvToBag(this);});
    }
    public void UpdateInfo(CardInventory.CardInfo cardInfo){
        cardName.text=cardInfo.card.name;
        cardCount.text=cardInfo.count.ToString();
        this.cardInfo=cardInfo;
        cardImg.sprite=cardInfo.card.image;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CardSelectManager.inst.cardTips.ShowTip(cardInfo.card);
    }
}