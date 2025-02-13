using UnityEngine;

public class E_Monkey : EnemyBase_Air{
    [Header("Attack")]
    public float flySpd;
    public float flyDuration;

    internal override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateDir();
    }
}