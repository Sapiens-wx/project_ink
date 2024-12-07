using UnityEngine;

public abstract class EnemyBase_Air : MobBase
{
    [Header("Idle State")]
    public int restDir; //is it upside down or upside up. 1 for up and -1 for down
    internal override void Start(){
        base.Start();
    }
}