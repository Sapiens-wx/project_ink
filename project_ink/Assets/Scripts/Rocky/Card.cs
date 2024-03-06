using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour
{
    public Sprite image;
    public int damage;

    public abstract void OnEnterSlot();
    public abstract void OnShot();
    public abstract void SetInk();
}
