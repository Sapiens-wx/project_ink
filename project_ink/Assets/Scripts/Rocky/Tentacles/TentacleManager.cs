using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Pool;

public class TentacleManager : Singleton<TentacleManager>
{
    [SerializeField] Book prefab_book;
    [SerializeField] AnimationClip clip_attack, clip_recover;
    /// <summary>
    /// used by OnCardDealDamage
    /// </summary>
    [SerializeField] float attackDistNear, attackDistFar, attackDistVeryFar;
    [Header("Follow Player")]
    /// <summary>
    /// the tentacles should keep the distance to the player within [followRadius]
    /// </summary>
    public float followRadius;
    public float acceleration, damping;

    /// <summary>
    /// the one that cards use to attack enemy. not inside a book
    /// </summary>
    [NonSerialized][HideInInspector] public Tentacle tentacle;
    /// <summary>
    /// calculated by
    /// </summary>
    [NonSerialized][HideInInspector] public float attackDuration;
    /// <summary>
    /// used by Card_T_6
    /// </summary>
    [NonSerialized][HideInInspector] public bool canReconcile;
    /// <summary>
    /// number of times player deals damage to an enemy (will be reset by OnCardDealDamage)
    /// </summary>
    int rank_damageCount=0;
    /// <summary>
    /// number of continues card cycles where the rank is equal to 3
    /// </summary>
    int rank3cycles=0;
    /// <summary>
    /// 苏醒程度
    /// </summary>
    int rank;
    public int Rank{
        get=>rank;
    }
    public int BookCount{
        get=>books.Count;
    }
    ObjectPool<Book> pool_book;
    [NonSerialized][HideInInspector] public List<Book> books;
    //temp variables
    Vector3 prevPos;
    void OnEnable(){
        CardEventManager.onCardDealDamage+=OnCardDealDamage;
        CardEventManager.onDistributeCard+=OnDistributeCard;
    }
    void OnDisable(){
        CardEventManager.onCardDealDamage-=OnCardDealDamage;
        CardEventManager.onDistributeCard-=OnDistributeCard;
    }
    // Start is called before the first frame update
    void Start()
    {
        pool_book=new ObjectPool<Book>(PoolBook_Create, PoolBook_OnGet, PoolBook_OnRelease);
        books=new List<Book>();
        attackDuration=clip_attack.length+clip_recover.length;
        rank=1;
        tentacle=PlayerCtrl.inst.tentacle;
    }
    void FixedUpdate(){
        FollowPlayer();
    }
    void FollowPlayer(){
        Vector3 tmp=transform.position;
        Vector3 dir=PlayerCtrl.inst.transform.position-tmp;
        Vector3 a=Vector3.zero;
        float dist=dir.magnitude;
        if(dist>followRadius){
            dir/=dist;
            a=(dist-followRadius)*acceleration*dir;
        }
        transform.position+=(tmp-prevPos)*damping+a;
        prevPos=tmp;
    }
    void OnDistributeCard(){
        if(rank==3){
            ++rank3cycles;
            if(rank3cycles>=3) Reconcile(1);
        } else
            rank3cycles=0;
    }
    void OnCardDealDamage(HitEnemyInfo info){
        //enemy hit by "books" does not count
        if(info.hitType==HitEnemyInfo.HitType.Tentacle && info.transform!=tentacle.transform) return;
        ++rank_damageCount;
        switch(rank){
            case 1:
                if(rank_damageCount>=2){
                    rank_damageCount=0;
                    Attack(1);
                    SetAttackMaxDist(attackDistNear);
                }
                break;
            case 2:
                if(rank_damageCount>=2){
                    rank_damageCount=0;
                    Attack(2);
                    SetAttackMaxDist(attackDistFar);
                }
                break;
            case 3:
                rank_damageCount=0;
                Attack(2);
                SetAttackMaxDist(attackDistVeryFar);
                break;
        }
    }
    void SetAttackMaxDist(float dist){
        foreach(Book b in books){
            b.tentacle.maxLength=dist;
        }
    }
    public void Attack(int baseDamage){
        if(RoomManager.CurrentRoom==null)
            return;
        EnemyBase closestEnemy=RoomManager.CurrentRoom.ClosestEnemy(PlayerCtrl.inst.transform);
        if(closestEnemy!=null){
            Vector2 closestEnemyPos=closestEnemy.transform.position;
            int animIdx=UnityEngine.Random.Range(0,3); //get a random tentacle attack animation to play
            foreach(Book b in books){
                b.tentacle.Attack(closestEnemyPos, b.accumulatedDamage+baseDamage, animIdx);
            }
        }
    }
    //----------Buff----------
    /// <summary>
    /// if rank==3, enter crazy mode
    /// </summary>
    public Card CrazyFireCardMode(Card card, List<IEnumerator> actions){
        if(rank==3){
            switch(UnityEngine.Random.Range(0,4)){
                case 0:
                case 1: //50% probability: randomly select a card in a cardslot and fire
                    List<int> indices=new List<int>(CardSlotManager.inst.cardSlots.Length);
                    for(int i=CardSlotManager.inst.cardSlots.Length-1;i>-1;--i){
                        if(CardSlotManager.inst.cardSlots[i].card!=null) indices.Add(i);
                    }
                    return CardSlotManager.inst.cardSlots[indices[UnityEngine.Random.Range(0, indices.Count)]].card;
                case 2: //25% probability: fire the card one more time
                    actions.Add(card.Activate(false));
                    CardSlotManager.inst.cardDealer.ReturnToCardPool(card.Copy());
                    break;
                case 3:
                    card.IsConsumed=true;
                    break;
            }
        }
        return card;
    }
    //----------Modifier----------
    /// <summary>
    /// increase rank
    /// </summary>
    public void Pray(int n){
        rank+=n;
        if(rank>3) rank=3;
    }
    /// <summary>
    /// decrease rank
    /// </summary>
    public void Reconcile(int n){
        if(!canReconcile) return;
        rank-=n;
        if(rank<0) rank=0;
    }
    /// <summary>
    /// create n number of new tentacles
    /// </summary>
    public void AddNTentacles(int n){
        for(int i=0;i<n;++i){
            Book book=pool_book.Get();
            books.Add(book);
        }
    }
    public void RemoveATentacle(){
        pool_book.Release(books[^1]);
        books.RemoveAt(books.Count-1);
    }
    //----------object pool functions----------
    Book PoolBook_Create(){
        return Instantiate(prefab_book.gameObject, transform).GetComponent<Book>();
    }
    void PoolBook_OnGet(Book book){
        book.gameObject.SetActive(true);
    }
    void PoolBook_OnRelease(Book book){
        book.gameObject.SetActive(false);
    }
}
