using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardBagInfoUI : MonoBehaviour{
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
}