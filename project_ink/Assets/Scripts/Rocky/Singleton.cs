using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    internal virtual void Awake()
    {
        instance = (T)this;
    }
}
