using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Text;
using Unity.VisualScripting;

public class CardSlotManager : Singleton<CardSlotManager>
{
    public int numSlots;
    [SerializeField] CardInventory inventory;
    [SerializeField] Transform cardSlotGridLayoutGroup,slotPointer;
    [SerializeField] GameObject cardSlotPrefab;
    /// <summary>
    /// in degree
    /// </summary>
    [Header("Auto Fire")]
    [SerializeField] float autoFireAngleRange;
    [Header("UI")]
    [SerializeField] ProgressBar anticipationBar;
    public CardTips cardTips;

    [HideInInspector] public CardSlot[] cardSlots;
    [HideInInspector] public CardDealer cardDealer;
    private int curSlot;
    private bool anticipating;
    private Vector2 shootDir; //the direction to shoot the card;
    //toggle panel
    bool toggle_panel=true;
    float toggle_panel_ypos;
    private List<Card> autoFireCards, autoActivateCards;
    //-----card effects-----
    public Buff_ReduceAntic buff1_3;
    public Buff1_4 buff1_4;
    public Buff1_5 buff1_5;
    public PlanetBuff planetBuff;
    public Buff_ReduceAntic buffP_2;
    public BuffP_3 buffP_3;
    public BuffP_5 buffP_5;
    public BuffP_6 buffP_6;
    public BuffP_7 buffP_7;
    public BuffP_8 buffP_8;

    private void Start()
    {
        //toggle card panel
        toggle_panel_ypos=transform.position.y;
        autoFireCards=new List<Card>();
        autoActivateCards=new List<Card>();
        InitializeCardSlotUI();
        UpdateBagCards();
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab))
            TogglePanel();
    }
    void InitializeCardSlotUI(){
        curSlot = 0;
        cardSlots = new CardSlot[numSlots];
        for(int i = 0; i < numSlots; ++i)
        {
            CardSlot slot = Instantiate(cardSlotPrefab).GetComponent<CardSlot>();
            slot.transform.SetParent(cardSlotGridLayoutGroup, false);
            cardSlots[i] = slot;
            slot.index = i;
        }
    }
    public void UpdateBagCards(){
        for(int i=0;i<numSlots;++i){
            cardSlots[i].card=null;
        }
        cardDealer = new CardDealer(inventory.bagRuntime);
        DistributeCard();
    }
    #region Card Mechanics
    IEnumerator Anticipate(){
        anticipating=true;
        //implement card1_3
        float totalTime=buff1_3.Activate(cardSlots[curSlot].card.anticipation);
        //planet Mercury effect
        totalTime=planetBuff.Mercury(totalTime);
        //planet Mercury activate effect
        totalTime=buffP_2.Activate(totalTime);

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
    public void AddAutoFireCard(Card card){
        card.ReturnToCardPool();
        autoFireCards.Add(card);
    }
    public void AddActivateCard(Card card){
        autoActivateCards.Add(card);
    }
    public void PrepareFire(Vector2 dir){
        if(anticipating) return;
        CardLog.MouseFire();
        //count cards
        int cardCnt=0;
        for(int i=0;i<cardSlots.Length;++i){
            if(cardSlots[i].card!=null) ++cardCnt;
        }
        CardLog.Log($"CNT {cardCnt} {cardDealer.DiscardPileCount()}");
        //shoot a card
        this.shootDir=dir;
        List<IEnumerator> actions=new List<IEnumerator>();
        Card firingCard=cardSlots[curSlot].card;
        firingCard = TentacleManager.inst.CrazyFireCardMode(firingCard, actions);
        firingCard.Prep_Fire(actions);
        StartCoroutine(Fire(actions));
    }
    public void SkipCard(){
        if(anticipating) return;
        IncCurSlot();
        PlayerCtrl.inst.EnableBlackDash(); //can black dash after skipping card
    }
    IEnumerator Fire(List<IEnumerator> actions){
        anticipating=true;
        foreach(IEnumerator ienum in actions){
            while(ienum.MoveNext()){
                yield return ienum.Current;
            }
            yield return new WaitForFixedUpdate();
        }
        //fire auto fired cards
        int cardNum=autoFireCards.Count+autoActivateCards.Count;
        float halfRangeInRad=Mathf.Deg2Rad*autoFireAngleRange*.5f;
        float deltaTheta=-halfRangeInRad*2/cardNum;
        Vector2 dir=MathUtil.Rotate(shootDir, halfRangeInRad+deltaTheta/2);
        foreach(Card card in autoFireCards){
            Projectile p=card.FireCard(Projectile.ProjectileType.AutoFire, false); //don't return it to card pool because this card is already returned to card pool when it is added to autoFiredCards.
            if(p==null) continue; //p is tentacle group
            p.AdjustFlyDir(dir);
            dir=MathUtil.Rotate(dir,deltaTheta);
        }
        foreach(Card card in autoActivateCards){
            Projectile p=card.FireCard(Projectile.ProjectileType.AutoFire, false);
            if(p==null) continue; //p is tentacle group
            p.AdjustFlyDir(dir);
            dir=MathUtil.Rotate(dir,deltaTheta);
        }
        autoFireCards.Clear();
        autoActivateCards.Clear();
        IncCurSlot();
    }
    /// <summary>
    /// Instantiate a projectile with parameters given in the card parameter
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public Projectile InstantiateProjectile(Card card, Projectile.ProjectileType type)
    {
        Projectile p = ProjectileManager.inst.CreateProjectile();
        p.AdjustRotation(shootDir);
        p.InitProjectile(card, PlayerShootingController.inst.transform.position, shootDir*ProjectileManager.inst.projectileSpeed, type);
        return p;
    }
    public Projectile InstantiateProjectile(int damage, Projectile.ProjectileType type)
    {
        Projectile p = ProjectileManager.inst.CreateProjectile();
        p.AdjustRotation(shootDir);
        p.InitProjectile(damage, PlayerShootingController.inst.transform.position, shootDir*ProjectileManager.inst.projectileSpeed, type);
        return p;
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
        if(card==null) return; //not enough cards. will not deal card to slot
        if(cardSlots[slot].card!=null){
            Debug.LogError("In assignCardToSlot(), there is already a card in the slot");
            CardLog.Log($"assign {card.type} to [{slot}], but already has a card");
            card.ReturnToCardPool();
            return;
        } else CardLog.Log($"assign {card.type} to [{slot}]");
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
        buff1_4.firstCardOfRound=true;
        CardEventManager.onDistributeCard?.Invoke();
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
    #endregion
    void TogglePanel(){
        toggle_panel=!toggle_panel;
        if(toggle_panel)
            transform.DOMoveY(toggle_panel_ypos, .3f);
        else transform.DOMoveY(toggle_panel_ypos-550, .3f);
    }
    public StringBuilder CurCardSlotState(){
        StringBuilder sb=new StringBuilder();
        for(int i=0;i<cardSlots.Length;++i){
            if(cardSlots[i].card==null)
                sb.Append("null | ");
            else
                sb.Append(cardSlots[i].card.type+" | ");
        }
        return sb;
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

    public CardDealer(CardInventory.CardInfo[] initialCards)
    {
        allCards = new List<Card>(initialCards.Length);
        foreach(CardInventory.CardInfo info in initialCards){
            if(info==null) continue;
            for(int i=0;i<info.count;++i){
                Card card = info.card.Copy();
                card.ResetRuntimeParams(this);
                allCards.Add(card);
            }
        }
        discardCardPile = new List<Card>(allCards);
    }
    /// <returns>true if can deal a card. false if no cards left that can be dealt</returns>
    public bool CanGetCard(){
        return discardCardPile.Count==0;
    }
    public Card GetCard()
    {
        if (discardCardPile.Count == 0){
            Debug.LogWarning("not enough card to be dealt. return null");
            return null;
        }
        int rd = UnityEngine.Random.Range(0, discardCardPile.Count);
        Card ret = discardCardPile[rd];
        discardCardPile[rd] = discardCardPile[discardCardPile.Count - 1];
        discardCardPile.RemoveAt(discardCardPile.Count - 1);
        ret.SlotIndex=-2; //-2 means the card is dealt but is not yet assigned to a slot. this is to avoid the case where a card is dealt to a slot, but the slot already has a card, so return the dealt card. if slotIndex==-1, then there will be an error

        CardLog.DealCard(ret);
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
        CardLog.ReturnCard(card);
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
    public int DiscardPileCount(){
        return discardCardPile.Count;
    }
    public int TotalCardCount(){
        return allCards.Count;
    }
}
