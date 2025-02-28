using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "card inventory", menuName = "Inventory/card inventory")]
public class CardInventory : ScriptableObject
{
    /// <summary>
    /// the card pile
    /// </summary>
    [SerializeField] List<CardInfo> cards;
    /// <summary>all the cards that the player has</summary>
    [SerializeField] List<CardInfo> inventory;

    [HideInInspector] public CardInfo[] invRuntime, bagRuntime;
    public void Init(){
        invRuntime=ArrToDictionary(inventory);
        bagRuntime=ArrToDictionary(cards);
    }
    [System.Serializable]
    public class CardInfo{
        public Card card;
        public int count;
        public CardInfo(){
        }
        public CardInfo(Card card){
            this.card=card;
            count=0;
        }
    }
    public CardInfo[] ArrToDictionary(List<CardInfo> cardInfos){
        CardInfo[] res=new CardInfo[(int)Card.CardType.Card_MaxCount];
        foreach(CardInfo info in cardInfos){
            res[(int)info.card.type]=info;
        }
        return res;
    }
    public void SaveRunTimeCards(){
        cards.Clear();
        inventory.Clear();
        foreach(CardInfo info in bagRuntime){
            if(info!=null && info.count>0)
                cards.Add(info);
        }
        foreach(CardInfo info in invRuntime){
            if(info!=null && info.count>0)
                inventory.Add(info);
        }
    }
}
