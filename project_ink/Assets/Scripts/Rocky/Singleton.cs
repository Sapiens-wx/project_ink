using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T inst;
    internal virtual void Awake()
    {
        inst = (T)this;
    }
}
