using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotManager : Singleton<CardSlotManager>
{
    public int numSlots;
    [SerializeField] CardInventory inventory;
    [SerializeField] Transform cardSlotGridLayoutGroup,slotPointer;
    [SerializeField] GameObject cardSlotPrefab;
    [Header("UI")]
    [SerializeField] ProgressBar anticipationBar;

    [HideInInspector] public CardSlot[] cardSlots;
    [HideInInspector] public CardDealer cardDealer;
    private int curSlot;
    private bool anticipating;
    //-----card effects-----
    //card_1_3
    [HideInInspector] public int anticReduceCount;
    [HideInInspector] public float anticReduceAmount;
    //card_1_6
    [HideInInspector] public int autoActivateOnDiscard;
    //card_1_5
    [HideInInspector] public int effect_card1_5=0;

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
        else if(Input.GetKeyDown(KeyCode.F)){
            SkipCard();
        }
    }
    IEnumerator Anticipate(){
        anticipating=true;
        float totalTime=cardSlots[curSlot].card.anticipation;
        //implement card1_3
        if(anticReduceCount>0){
            anticReduceCount--;
            totalTime*=anticReduceAmount;
        }

        float time=totalTime;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        anticipationBar.gameObject.SetActive(true);
        while(time>0){
            anticipationBar.SetProgress(time/totalTime);
            time-=Time.fixedDeltaTime;
            yield return wait;
        }
        anticipationBar.gameObject.SetActive(false);
        anticipating=false;
        yield break;
    }
    public void PrepareFire(){
        if(anticipating) return;
        List<IEnumerator> actions=new List<IEnumerator>();
        cardSlots[curSlot].card.Prep_Fire(actions);
        StartCoroutine(Fire(actions));
    }
    public void SkipCard(){
        if(anticipating) return;
        IncCurSlot();
    }
    IEnumerator Fire(List<IEnumerator> actions){
        anticipating=true;
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
        StartCoroutine(Anticipate()); //enter the anticipation of the card in the cardslot
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
            else
                SetCurSlot(curSlot);
        }
    }
    public void AssignCardToSlot(int slot, Card card)
    {
        if(cardSlots[slot].card!=null){
            Debug.LogError("In assignCardToSlot(), there is already a card in the slot");
            return;
        }
        card.OnEnterSlot(slot);
        cardSlots[slot].SetCard_Anim(card);
    }
    public void AssignCardToSlotRandomly(int slotIndex)
    {
        if(cardSlots[slotIndex].card!=null){
            return;
        }
        AssignCardToSlot(slotIndex, cardDealer.GetCard());
    }
    public IEnumerator AssignCardToSlotRandomly_ienum(int slotIndex){
        AssignCardToSlot(slotIndex, cardDealer.GetCard());
        yield break;
    }
    IEnumerator DistributeCard_Anim()
    {
        for(int i = 0; i < numSlots; ++i)
        {
            if (cardSlots[i].card == null)
            {
                AssignCardToSlotRandomly(i);
            }
        }
        SetCurSlot(0);
        yield break;
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
