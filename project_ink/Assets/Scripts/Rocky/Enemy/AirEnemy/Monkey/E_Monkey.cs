using UnityEngine;

public class E_Monkey : EnemyBase_Air{
    [Header("Attack")]
    public float flyAngluarSpd;
    public float flyDuration;

    public event System.Action<Collision2D> onCollisionEnter;
    void OnCollisionEnter2D(Collision2D collision){
        onCollisionEnter?.Invoke(collision);
    }
}