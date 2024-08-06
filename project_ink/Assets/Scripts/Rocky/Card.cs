using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : ScriptableObject
{
    public CardType type;
    public Sprite image;
    public int damage;

    //Runtime variables
    /// <summary>
    /// whether the card is consumed in one round
    /// </summary>
    internal bool isConsumed;
    /// <summary>
    /// whether the card is consumed in one round
    /// </summary>
    public bool IsConsumed { get { return isConsumed; } }
    /// <summary>
    /// 
    /// </summary>
    protected CardDealer cardDealer;
    /// <summary>
    /// index in CardSlotManager.cardSlots if applicable
    /// </summary>
    protected int slotIndex;

    /// <summary>
    /// reset parameters needed in runtime. For example: this is called when the player first enters a round.
    /// </summary>
    public virtual void ResetRuntimeParams(CardDealer dealer)
    {
        isConsumed = false;
        cardDealer = dealer;
    }
    protected virtual void ReturnToCardPool()
    {
        if (!isConsumed)
            cardDealer.ReturnToCardPool(this);
    }
    public virtual void OnEnterSlot(int slot) {
        slotIndex = slot;
    }
    /// <summary>
    /// called when the card is shot
    /// </summary>
    /// <param name="slot"></param>
    public virtual IEnumerator OnShot(CardSlot slot, System.Action callback) 
    {
        callback?.Invoke();
        ReturnToCardPool();
        yield break;
    }
    /// <summary>
    /// generates an automatic bullet flying toward an enemy
    /// </summary>
    /// <param name="slot"></param>
    public virtual IEnumerator OnActivate(CardSlot slot, System.Action callback) {
        yield break;
    }
    /// <summary>
    /// Destroy the card. The card never appears again in this round.
    /// </summary>
    public virtual void Consume()
    {
        isConsumed = true;
    }
    /// <summary>
    /// Discard the card into discard card pile. the card will be dealt again in this round
    /// </summary>
    public virtual void OnDiscard()
    {
        ReturnToCardPool();
        CardSlotManager.instance.cardSlots[slotIndex].SetCard_Anim(null);
    }
    public abstract Card Copy();
    public virtual void CopyTo(Card card)
    {
        card.damage = damage;
        card.type = type;
        card.image = image;
    }
    public enum CardType
    {
        Card_1_1,
        Card_1_2,
        Card_1_3,
        Card_1_4,
    }
}
