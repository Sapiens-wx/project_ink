using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotManager : Singleton<CardSlotManager>
{
    public int numSlots;
    [SerializeField] CardInventory inventory;
    [SerializeField] Transform cardSlotGridLayoutGroup,slotPointer;
    [SerializeField] GameObject cardSlotPrefab;

    [HideInInspector] public CardSlot[] cardSlots;
    [HideInInspector] public CardDealer cardDealer;
    private int curSlot;

    private void Start()
    {
        curSlot = 0;
        cardSlots = new CardSlot[numSlots];
        for(int i = 0; i < numSlots; ++i)
        {
            CardSlot slot = Instantiate(cardSlotPrefab).GetComponent<CardSlot>();
            slot.transform.SetParent(cardSlotGridLayoutGroup, false);
            cardSlots[i] = slot;
            slot.index = i;
        }
        cardDealer = new CardDealer(inventory.cards);
        DistributeCard();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PrepareFire();
        }
    }
    public void PrepareFire(){
        List<IEnumerator> actions=new List<IEnumerator>();
        cardSlots[curSlot].card.Prep_Fire(actions);
        StartCoroutine(Fire(actions));
    }
    IEnumerator Fire(List<IEnumerator> actions){
        foreach(IEnumerator ienum in actions){
            while(ienum.MoveNext()){
                yield return ienum.Current;
            }
            yield return new WaitForFixedUpdate();
        }
        IncCurSlot();
    }
    /// <summary>
    /// Instantiate a projectile with parameters given in the card parameter
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public GameObject InstantiateProjectile(Card card)
    {
        return null;
    }
    public Card ActivateCard(int index, System.Action callback)
    {
        Card ret = cardSlots[index].card;
        cardSlots[index].card.OnActivate(cardSlots[index], callback);
        return ret;
    }
    private void DistributeCard()
    {
        StartCoroutine(DistributeCard_Anim());
    }
    public int GetCurSlot()
    {
        return curSlot;
    }
    public void SetCurSlot(int cur)
    {
        curSlot = cur;
        UpdateCurSlot();
    }
    private void UpdateCurSlot()
    {
        slotPointer.position = cardSlots[curSlot].transform.position;
    }
    /// <summary>
    /// move to the next non-empty slot
    /// </summary>
    public void IncCurSlot()
    {
        if (curSlot == numSlots - 1) DistributeCard();
        else
        {
            for (++curSlot; curSlot + 1 < numSlots && cardSlots[curSlot].card == null; ++curSlot) ;
            if (cardSlots[curSlot].card == null) DistributeCard();
            else UpdateCurSlot();
        }
    }
    public void AssignCardToSlot(int slot, Card card)
    {
        card.OnEnterSlot(slot);
        cardSlots[slot].SetCard_Anim(card);
    }
    public void AssignCardToSlotRandomly(int slotIndex)
    {
        AssignCardToSlot(slotIndex, cardDealer.GetCard());
    }
    public IEnumerator AssignCardToSlotRandomly_ienum(int slotIndex){
        AssignCardToSlot(slotIndex, cardDealer.GetCard());
        yield break;
    }
    IEnumerator DistributeCard_Anim()
    {
        SetCurSlot(0);
        for(int i = 0; i < numSlots; ++i)
        {
            if (cardSlots[i].card == null)
            {
                AssignCardToSlotRandomly(i);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}

public class CardDealer
{
    /// <summary>
    /// all the cards in this round.
    /// </summary>
    private List<Card> allCards;
    /// <summary>
    /// cards waiting to be shuffled
    /// </summary>
    private List<Card> discardCardPile;
    /// <summary>
    /// shuffled cards
    /// </summary>

    public CardDealer(List<Card> initialCards)
    {
        allCards = new List<Card>(initialCards.Count);
        for (int i = 0; i < initialCards.Count; ++i)
        {
            Card card = initialCards[i].Copy();
            card.ResetRuntimeParams(this);
            allCards.Add(card);
        }
        discardCardPile = new List<Card>(allCards);
    }
    public Card GetCard()
    {
        if (discardCardPile.Count == 0) throw new System.Exception("error in CardDealer.GetCards: does not have enough cards to be dealt");
        int rd = UnityEngine.Random.Range(0, discardCardPile.Count);
        Card ret = discardCardPile[rd];
        discardCardPile[rd] = discardCardPile[discardCardPile.Count - 1];
        discardCardPile.RemoveAt(discardCardPile.Count - 1);
        return ret;
    }
    public List<Card> GetCards(int num)
    {
        List<Card> ret = new List<Card>(num);
        int max = Mathf.Min(discardCardPile.Count, num);
		int i;
        for (i = 0; i < max; ++i)
        {
            ret.Add(GetCard());
        }
		if(i!=num){
            Debug.LogError("error in CardDealer.GetCards: does not have enough cards to be dealt");
		}
        return ret;
    }
    public void ReturnToCardPool(Card card)
    {
        discardCardPile.Add(card);
    }
    public void ConsumeCardsOfType(Card.CardType type)
    {
        for(int i=0;i< allCards.Count; ++i)
        {
            while (allCards.Count>0 && allCards[i].IsConsumed)
            {
                allCards[i] = allCards[allCards.Count - 1];
                allCards.RemoveAt(allCards.Count - 1);
            }
            if (allCards[i].type == type)
                allCards[i].Consume();
        }
    }
}
