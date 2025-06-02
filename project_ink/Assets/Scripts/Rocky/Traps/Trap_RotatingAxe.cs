using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Trap_RotatingAxe : TrapBase
{
    /// <summary>
    /// angularSpd is in degree/sec
    /// </summary>
    [SerializeField] float angularSpd;

    Quaternion rotQuat;
    protected override void Start(){
        base.Start();
        rotQuat=Quaternion.Euler(new Vector3(0,0,angularSpd*Time.fixedDeltaTime));
    }
    void FixedUpdate(){
        transform.rotation=rotQuat*transform.rotation;
    }
}
