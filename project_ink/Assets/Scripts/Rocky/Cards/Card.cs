using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    public CardType type;
    public CardGroup group;
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
        OnExitSlot();
        if (!isConsumed)
            cardDealer.ReturnToCardPool(this);
    }
    public virtual void OnEnterSlot(int slot) {
        slotIndex = slot;
    }
    public void OnExitSlot(){
        slotIndex=-1;
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
        //Mars effect
        CardSlotManager.inst.planetBuff.Mars(this, actions);
        //Mars activation effect
        CardSlotManager.inst.buffP_5.Activate(this, actions);
        //Venus effect
        CardSlotManager.inst.planetBuff.Venus(this, actions);
        //Venus activation effect
        CardSlotManager.inst.buffP_3.Activate(this, actions);
        //Jupiter effect
        CardSlotManager.inst.planetBuff.Jupiter(actions);
        //Jupiter activation effect
        CardSlotManager.inst.buffP_7.Activate(actions);
    }
    public virtual void Prep_Discard(List<IEnumerator> actions){
        actions.Add(Discard());
    }
    internal IEnumerator Fire(){
        CardSlotManager.inst.cardSlots[slotIndex].SetCard_Anim(null);
        CardSlotManager.inst.InstantiateProjectile(this, false);
        ReturnToCardPool();
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator AutoFire(){
        if(slotIndex>=0)
            CardSlotManager.inst.cardSlots[slotIndex].SetCard_Anim(null);
        CardSlotManager.inst.InstantiateProjectile(this, true);
        ReturnToCardPool();
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator Activate(){
        CardSlotManager.inst.InstantiateProjectile(this, true);
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator OnDiscardBuffCheck(){
        //buff1_4
        IEnumerator ienum=CardSlotManager.inst.buff1_4.Activate(Activate());
        while(ienum.MoveNext())
            yield return ienum.Current;
        //buff1_5
        CardSlotManager.inst.buff1_5.Activate();
    }
    internal IEnumerator Discard(){
        IEnumerator ienum=OnDiscardBuffCheck();
        while(ienum.MoveNext())
            yield return ienum.Current;

        CardSlotManager.inst.cardSlots[slotIndex].SetCard_Anim(null);
        ReturnToCardPool();
        yield break;
    }
    public virtual void OnHitEnemy(EnemyBase enemy){}
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

        Card_P_Earth,
        Card_P_Mercury,
        Card_P_Venus,
        Card_P_Uranus,
        Card_P_Mars,
        Card_P_Sun,
        Card_P_Jupiter,
        Card_P_Saturn,

        Card_MaxCount
    }
    public enum CardGroup{
        Discard,
        Planet
    }
}