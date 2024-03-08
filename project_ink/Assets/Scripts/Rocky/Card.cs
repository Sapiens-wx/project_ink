using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : ScriptableObject
{
    public Sprite image;
    public int damage;

    public abstract void OnEnterSlot(CardSlot slot);
    public abstract void OnShot(CardSlot slot);
    public abstract void SetInk();
}
