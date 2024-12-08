using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Diamond : EnemyBase_Ground
{
    [Header("Attack")]
    public float shootAngle; //in degree
    public float shootAngleRange;
    public float shootForce, shootForceRange;
}
