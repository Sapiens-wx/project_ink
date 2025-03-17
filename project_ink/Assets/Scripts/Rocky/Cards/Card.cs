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
    [TextArea] public string explanation;

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
    public int SlotIndex{
        set=>slotIndex=value;
        get=>slotIndex;
    }

    /// <summary>
    /// reset parameters needed in runtime. For example: this is called when the player first enters a round.
    /// </summary>
    public virtual void ResetRuntimeParams(CardDealer dealer)
    {
        isConsumed = false;
        cardDealer = dealer;
    }
    public virtual void ReturnToCardPool()
    {
        if (!isConsumed){
            if(slotIndex>=0)
                CardSlotManager.inst.cardSlots[slotIndex].SetCard_Anim(null);
            cardDealer.ReturnToCardPool(this);
            OnExitSlot();
        }
    }
    public virtual void OnEnterSlot(int slot) {
        slotIndex = slot;
    }
    public void OnExitSlot(){
        if(slotIndex==-1) Debug.LogError("returning a card that is already returned");
        if(slotIndex==-1) CardLog.Log($"Error: returning a card that is already returned");
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
    /// <summary>
    /// fired and auto fired card returns to card pool. activated card does not
    /// </summary>
    /// <param name="autoChase"></param>
    /// <param name="returnToCardPool"></param>
    /// <returns></returns>
    public virtual Projectile FireCard(Projectile.ProjectileType type, bool returnToCardPool){
        Projectile p = CardSlotManager.inst.InstantiateProjectile(this, type);
        if(returnToCardPool)
            ReturnToCardPool();
        return p;
    }
    internal IEnumerator Fire(){
        CardLog.LogFire(this);
        FireCard(Projectile.ProjectileType.Normal, true);
        yield return new WaitForSeconds(recovery);
    }
    /// <summary>
    /// Auto Fire the card
    /// </summary>
    /// <param name="fireInGroup">cards auto fired by effects from the discard group should set fireInGroup to true. Otherwise set it to false</param>
    /// <returns></returns>
    internal IEnumerator AutoFire(bool fireInGroup){
        CardLog.LogAutoFire(this);
        if(fireInGroup)
            CardSlotManager.inst.AddAutoFireCard(this);
        else
            FireCard(Projectile.ProjectileType.AutoFire, true);
        yield return new WaitForSeconds(recovery);
    }
    /// <param name="fireInGroup">cards auto fired by effects from the discard group should set fireInGroup to true. Otherwise set it to false</param>
    internal IEnumerator Activate(bool fireInGroup){
        if(fireInGroup)
            CardSlotManager.inst.AddActivateCard(this);
        else
            FireCard(Projectile.ProjectileType.AutoFire, false);
        yield return new WaitForSeconds(recovery);
    }
    internal IEnumerator OnDiscardBuffCheck(){
        //buff1_4
        IEnumerator ienum=CardSlotManager.inst.buff1_4.Activate(Activate(true));
        while(ienum.MoveNext())
            yield return ienum.Current;
        //buff1_5
        CardSlotManager.inst.buff1_5.Activate();
    }
    internal IEnumerator Discard(){
        IEnumerator ienum=OnDiscardBuffCheck();
        while(ienum.MoveNext())
            yield return ienum.Current;

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
        card.explanation=explanation;
    }
    public enum CardType
    {
        //discard
        Card_1_1,
        Card_1_2,
        Card_1_3,
        Card_1_4,
        Card_1_5,
        Card_1_6,
        Card_1_7,
        //planet
        Card_P_Earth,
        Card_P_Mercury,
        Card_P_Venus,
        Card_P_Uranus,
        Card_P_Mars,
        Card_P_Sun,
        Card_P_Jupiter,
        Card_P_Saturn,
        //normal
        Card_N_Dmg1,
        Card_N_Dmg2,
        Card_N_Discard,
        //tentacle
        Card_T_1,

        Card_MaxCount
    }
    public enum CardGroup{
        Discard,
        Planet,
        Normal,
        Tentacle
    }
}