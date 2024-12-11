using UnityEngine;

public class E_Heart:EnemyBase_Ground{
    [Header("Attack")]
    public int recoverAmount;
    public float recoverRange;
    public float recoverDuration;
    internal override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(transform.position,recoverRange);
    }
}