using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInvInfoUI : MonoBehaviour{
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
}