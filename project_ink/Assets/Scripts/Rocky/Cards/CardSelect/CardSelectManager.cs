using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectManager : Singleton<CardSelectManager>
{
    [SerializeField] GameObject cardSelectionPanel;
    [SerializeField] CardInvInfoUI cardInvInfoPrefab;
    [SerializeField] CardBagInfoUI cardBagInfoPrefab;
    [SerializeField] Transform invPanel, bagPanel;
    public CardTips cardTips;

    CardInventory inv;
    void Start(){
        inv=GameManager.inst.cardInventory;
        UpdateInvPanel();
        UpdateBagPanel();
        cardSelectionPanel.SetActive(false);
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            cardSelectionPanel.SetActive(!cardSelectionPanel.activeSelf);
            if(cardSelectionPanel.activeSelf==false){
                CardSlotManager.inst.UpdateBagCards();
                CardZoomInTip.HideTip();
            }
        }
    }
    public void FromInvToBag(CardInvInfoUI cardInvInfoUI){
        CardInventory.CardInfo cardBagInfo=inv.bagRuntime[(int)cardInvInfoUI.cardInfo.card.type];
        //if cardInvInfo hasn't been created, create one
        if(cardBagInfo==null){
            cardBagInfo=new CardInventory.CardInfo(cardInvInfoUI.cardInfo.card);
            inv.bagRuntime[(int)cardInvInfoUI.cardInfo.card.type]=cardBagInfo;
        }
        //increase cardInvInfo's count, decrease cardBagInfo's count.
        cardBagInfo.count++;
        cardInvInfoUI.cardInfo.count--;
        UpdateInvPanel();
        UpdateBagPanel();
    }
    public void FromBagToInv(CardBagInfoUI cardBagInfoUI){
        CardInventory.CardInfo cardInvInfo=inv.invRuntime[(int)cardBagInfoUI.cardInfo.card.type];
        //if cardInvInfo hasn't been created, create one
        if(cardInvInfo==null){
            cardInvInfo=new CardInventory.CardInfo(cardBagInfoUI.cardInfo.card);
            inv.invRuntime[(int)cardBagInfoUI.cardInfo.card.type]=cardInvInfo;
        }
        //increase cardInvInfo's count, decrease cardBagInfo's count.
        cardInvInfo.count++;
        cardBagInfoUI.cardInfo.count--;
        UpdateInvPanel();
        UpdateBagPanel();
    }
    void UpdateInvPanel(){
        int count=0;
        for(int i=0;i<inv.invRuntime.Length;++i)
            if(inv.invRuntime[i]!=null && inv.invRuntime[i].count>0)
                count++;
        //make sure invPanel has enough children
        for(int i=invPanel.childCount;i<count;++i)
            Instantiate(cardInvInfoPrefab.gameObject).transform.SetParent(invPanel);
        //make sure the overflowed children are setactive to false
        for(int i=count;i<invPanel.childCount;++i)
            invPanel.GetChild(i).gameObject.SetActive(false);
        for(int i=0,child=0;i<inv.invRuntime.Length;++i){
            CardInventory.CardInfo cardInfo=inv.invRuntime[i];
            if(cardInfo!=null&&cardInfo.count>0){
                CardInvInfoUI ui=invPanel.GetChild(child++).GetComponent<CardInvInfoUI>();
                ui.gameObject.SetActive(true);
                ui.UpdateInfo(cardInfo);
            }
        }
    }
    void UpdateBagPanel(){
        int count=0;
        for(int i=0;i<inv.bagRuntime.Length;++i)
            if(inv.bagRuntime[i]!=null && inv.bagRuntime[i].count>0)
                count++;
        //make sure bagPanel has enough children
        for(int i=bagPanel.childCount;i<count;++i)
            Instantiate(cardBagInfoPrefab.gameObject).transform.SetParent(bagPanel);
        //make sure the overflowed children are setactive to false
        for(int i=count;i<bagPanel.childCount;++i)
            bagPanel.GetChild(i).gameObject.SetActive(false);
        for(int i=0,child=0;i<inv.bagRuntime.Length;++i){
            CardInventory.CardInfo cardInfo=inv.bagRuntime[i];
            if(cardInfo!=null&&cardInfo.count>0){
                CardBagInfoUI ui=bagPanel.GetChild(child++).GetComponent<CardBagInfoUI>();
                ui.gameObject.SetActive(true);
                ui.UpdateInfo(cardInfo);
            }
        }
    }
}
