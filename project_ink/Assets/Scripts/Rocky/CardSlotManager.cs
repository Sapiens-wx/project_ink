using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotManager : Singleton<CardSlotManager>
{
    [SerializeField] int numSlots;
    [SerializeField] CardInventory inventory;
    [SerializeField] Transform cardSlotGridLayoutGroup;
    [SerializeField] GameObject cardSlotPrefab;

    CardSlot[] cardSlots;
    [HideInInspector] public CardPool cardPool;

    private void Start()
    {
        for(int i = 0; i < numSlots; ++i)
        {
            CardSlot slot = Instantiate(cardSlotPrefab).GetComponent<CardSlot>();
            slot.transform.SetParent(cardSlotGridLayoutGroup, false);
            cardSlots[i] = slot;
        }
        cardPool = new CardPool(inventory.cards);
    }
    private void DistributeCard()
    {
        List<Card> cards = cardPool.GetCards(numSlots);
        for(int i = 0; i < cards.Count; ++i)
        {
            cardSlots[i].SetCard(cards[i]);
            cards[i].OnEnterSlot();
        }
    }
}

public class CardPool
{
    public List<Card> initialCards;
    public Queue<Card> queue;

    public CardPool(List<Card> initialCards)
    {
        this.initialCards = initialCards;
    }
    public List<Card> GetCards(int num)
    {
        List<Card> ret = new List<Card>(num);
        int max = Mathf.Max(queue.Count, num);
        for (int i = 0; i < max; ++i)
        {
            ret.Add(queue.Dequeue());
        }
        if (queue.Count == 0)
        {
            Shuffle();
        }
        return ret;
    }
    private void Shuffle()
    {
        List<int> indices = new List<int>(initialCards.Count);
        for(int i=0; i < indices.Count; ++i)
        {
            indices.Add(i);
        }
        int j;
        while(indices.Count>0)
        {
            j = Random.Range(0, indices.Count - 1);
            queue.Enqueue(initialCards[j]);
            indices.RemoveAt(j);
        }
    }
}
