using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_MagicFire : TrapBase
{
    public static List<Trap_MagicFire> instances;
    [HideInInspector] public Collider2D bc;
    void Awake(){
        if(instances==null)
            instances=new List<Trap_MagicFire>();
    }
    public override void ChangeTheme(Theme theme)
    {
        base.ChangeTheme(theme);
        bc=activeSprite.GetComponent<Collider2D>();
    }
    void OnEnable(){
        instances.Add(this);
    }
    void OnDisable(){
        int idx=instances.IndexOf(this);
        instances[idx]=instances[instances.Count-1];
        instances.RemoveAt(instances.Count-1);
    }
}
