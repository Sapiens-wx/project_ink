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
    /// the one that cards use to attack enemy. not inside a book
    /// </summary>
    public Tentacle tentacle;

    /// <summary>
    /// calculated by
    /// </summary>
    [NonSerialized][HideInInspector] public float attackDuration;
    /// <summary>
    /// used by Card_T_6
    /// </summary>
    [NonSerialized][HideInInspector] public bool canReconcile;
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
    // Start is called before the first frame update
    void Start()
    {
        pool_book=new ObjectPool<Book>(PoolBook_Create, PoolBook_OnGet, PoolBook_OnRelease);
        books=new List<Book>();
        attackDuration=clip_attack.length+clip_recover.length;
    }
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
