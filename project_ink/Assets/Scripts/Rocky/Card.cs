using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : ScriptableObject
{
    public CardType type;
    public Sprite image;
    [TextArea] public string description;
    public int damage;
    public float anticipation, recovery;

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
    private float anticipation_backup;

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
    public virtual void Prep_Fire(List<IEnumerator> actions){
        actions.Add(Fire());
    }
    public virtual void Prep_Discard(List<IEnumerator> actions){
        actions.Add(Discard());
    }
    internal IEnumerator Fire(){
        CardSlotManager.instance.cardSlots[slotIndex].SetCard_Anim(null);
        CardSlotManager.instance.InstantiateProjectile(this);
        ReturnToCardPool();
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator AutoFire(){
        CardSlotManager.instance.cardSlots[slotIndex].SetCard_Anim(null);
        CardSlotManager.instance.InstantiateProjectile(this);
        ReturnToCardPool();
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator Activate(){
        CardSlotManager.instance.InstantiateProjectile(this);
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator Discard(){
        //buff1_4
        IEnumerator ienum=CardSlotManager.instance.buff1_4.Activate(Activate());
        while(ienum.MoveNext())
            yield return ienum.Current;
        //buff1_5
        CardSlotManager.instance.buff1_5.Activate();

        CardSlotManager.instance.cardSlots[slotIndex].SetCard_Anim(null);
        ReturnToCardPool();
        yield break;
    }
    public abstract Card Copy();
    public virtual void CopyTo(Card card)
    {
        card.damage = damage;
        card.type = type;
        card.image = image;
        card.anticipation=anticipation;
        card.recovery=recovery;
        card.description=description;
    }
    public enum CardType
    {
        Card_1_1,
        Card_1_2,
        Card_1_3,
        Card_1_4,
        Card_1_5,
        Card_1_6,
        Card_1_7,
    }
}
