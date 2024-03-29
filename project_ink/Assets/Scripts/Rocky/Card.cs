using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : ScriptableObject
{
    public CardType type;
    public Sprite image;
    public int damage;

    public virtual void OnEnterSlot(CardSlot slot) { }
    public virtual void OnShot(CardSlot slot) { }
    public virtual void SetInk() { }
    public enum CardType
    {
        Attack,
        CriticalHit,
        Fast,
        Freeze,
        BreakThrough,
        Hurricane,
        Connect
    }
}
