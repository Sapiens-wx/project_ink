using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Balloon : EnemyBase
{
    public event System.Action onDead;
    public override void OnHit(Projectile proj)
    {
        base.OnHit(proj);
        if(CurHealth==0){
            Destroy(gameObject);
            onDead?.Invoke();
        }
    }
}
