using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//colliders with this component can be hit by projectile.
//(in some cases like E_Pig, the collider of the parent object should not be hit by projectiles). so add this component to an object that act as the hit box for an enemy
[RequireComponent(typeof(Collider2D))]
public class EnemyHitBox : EnemyBase
{
    public EnemyBase connectedEnemy;
    Collider2D m_collider;
    // Start is called before the first frame update
    internal override void Start(){
    }
    protected override void OnDestroy(){
    }
    public override void OnHit(HitEnemyInfo proj)
    {
        connectedEnemy.OnHit(proj);
    }
}