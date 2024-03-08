using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotManager : Singleton<CardSlotManager>
{
    public int numSlots;
    [SerializeField] CardInventory inventory;
    [SerializeField] Transform cardSlotGridLayoutGroup,slotPointer;
    [SerializeField] GameObject cardSlotPrefab;

    CardSlot[] cardSlots;
    [HideInInspector] public CardPool cardPool;
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
        cardPool = new CardPool(inventory.cards);
        DistributeCard();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Shoot();
        }
    }
    private void DistributeCard()
    {
        List<Card> cards = cardPool.GetCards(numSlots);
        int i;
        for(i = 0; i < numSlots; ++i)
        {
            cardSlots[i].SetCard(cards[i]);
            cards[i].OnEnterSlot(cardSlots[i]);
        }
        curSlot = 0;
        UpdateCurSlot();
    }
    public int GetCurSlot()
    {
        return curSlot;
    }
    private void UpdateCurSlot()
    {
        slotPointer.position = cardSlots[curSlot].transform.position;
    }
    public Card Shoot()
    {
        CardSlot slot = cardSlots[curSlot];
        Card card = slot.card;
        cardSlots[curSlot].SetCard(null);

        ++curSlot;
        if (curSlot >= numSlots)
            DistributeCard();
        UpdateCurSlot();

        card.OnShot(slot);

        return card;
    }
}

public class CardPool
{
	public List<Card> cardPool;
    public Queue<Card> queue;
	
	public List<Card> pendingCards; //waiting to be added into cardPool

    public CardPool(List<Card> initialCards)
    {
        cardPool=new List<Card>(initialCards);
        queue = new Queue<Card>();
        pendingCards = new List<Card>();
        Shuffle();
    }
    public List<Card> GetCards(int num)
    {
        List<Card> ret = new List<Card>(num);
        int max = Mathf.Min(queue.Count, num);
		foreach(Card c in pendingCards){
			cardPool.Add(c);
		}
		int i;
        for (i = 0; i < max; ++i)
        {
            ret.Add(queue.Dequeue());
        }
		if(i!=num){
			Shuffle();
			for(;i<num;++i){
				ret.Add(queue.Dequeue());
			}
		}
        if (queue.Count == 0)
        {
            Shuffle();
        }
		pendingCards=new List<Card>(ret);
        return ret;
    }
    private void Shuffle()
    {
        List<int> indices = new List<int>(cardPool.Count);
        for(int i=0; i < cardPool.Count; ++i)
        {
            indices.Add(i);
        }
        int j;
        while(indices.Count>0)
        {
            j = Random.Range(0, indices.Count - 1);
            queue.Enqueue(cardPool[indices[j]]);
            indices.RemoveAt(j);
        }
        cardPool.Clear();
    }
}
