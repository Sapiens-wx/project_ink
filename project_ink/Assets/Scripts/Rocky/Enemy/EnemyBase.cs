using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] internal int maxHealth;
    internal int curHealth;
    public abstract void OnHit(Projectile proj);
}
