using System.Collections.Generic;
using UnityEngine;

public class E_CardCommander : EliteBase_Ground
{
    [Header("Action 1")]
    public float chargeInterval;
    public float chargeInterval2;
    public float airChargeInterval;
    public float jumpXDist;
    public float jumpYDist;
    public float dashSpd;
    public float dashDist;

    [HideInInspector] public int ac1_dash_count=0;
    [HideInInspector] public float gravityScale;
}
