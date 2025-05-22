using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.U2D;
using UnityEditor.Experimental.GraphView;

public class Trap_Portal : TrapBase{
    [SerializeField] Trap_Portal linkedPortal;
    [SerializeField] float angle;
    [SerializeField] int upDir; //1: portalUpDir is portalDir rotated by 90deg. -1: rotated by -90 deg

    Queue<Collider2D> teleportedObjs;
    Collider2D bc;
    /// <summary>
    /// opposite to the open direction (normal). e.g., objs entering from the left will be teleported, then portalDir=right (1,0)
    /// </summary>
    Vector2 portalDir;
    Vector2 portalUpDir;
    Matrix3x3 teleportMatrix;
    void OnValidate(){
        float angle_rad=angle*Mathf.Deg2Rad;
        portalDir.x=Mathf.Cos(angle_rad);
        portalDir.y=Mathf.Sin(angle_rad);
        portalUpDir.x=upDir==1?-portalDir.y:portalDir.y;
        portalUpDir.y=upDir==1?portalDir.x:-portalDir.x;
        //update sprite rotation
        foreach(GameObject go in sprites){
            go.transform.localRotation=Quaternion.Euler(new Vector3(0,0,angle));
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color=Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position+portalDir);
        Gizmos.color=Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position+portalUpDir);
    }
    protected override void Start()
    {
        base.Start();
        OnValidate();
        teleportedObjs=new Queue<Collider2D>();
        UpdatePortalTransformMatrix();
    }
    void OnTriggerStay2D(Collider2D collider){
        foreach(var obj in teleportedObjs){
            if(collider==obj) return;
        }
        Rigidbody2D rgb=collider.attachedRigidbody;
        if(rgb==null) return;
        //teleport
        if(Vector2.Dot(portalDir,rgb.velocity)>.02f){
            TeleportObj(collider);
        }
    }
    public override void ChangeTheme(Theme theme)
    {
        base.ChangeTheme(theme);
        bc=activeSprite.GetComponent<Collider2D>();
    }
    void UpdatePortalTransformMatrix(){
        //x-axis: portalDir
        //y-axis: portalUpDir
        //calculate world2portal
        Matrix3x3 world2portal=new Matrix3x3(new Vector3(portalDir.x, portalUpDir.x, 0),
            new Vector3(portalDir.y,portalUpDir.y,0),
            new Vector3(0,0,1));
        Vector3 offset=world2portal.Mul(-transform.position,0);
        world2portal.m02=offset.x;
        world2portal.m12=offset.y;
        //calculate portal2world
        Matrix3x3 portal2world=new Matrix3x3(new Vector3(-linkedPortal.portalDir.x,linkedPortal.portalUpDir.x,0),
            new Vector3(-linkedPortal.portalDir.y,linkedPortal.portalUpDir.y,0),
            new Vector3(linkedPortal.transform.position.x,linkedPortal.transform.position.y,1));
        //portal transform matrix
        teleportMatrix=portal2world*world2portal;
    }
    void TeleportObj(Collider2D collider){
        collider.transform.position=teleportMatrix.Mul(collider.transform.position,1);
        linkedPortal.teleportedObjs.Enqueue(collider);
        StartCoroutine(PopAfterSecs(.3f));
        //update rigidbody velocity
        if(collider.gameObject==PlayerCtrl.inst.gameObject){
            PlayerCtrl.inst.v_trap=teleportMatrix.Mul(collider.attachedRigidbody.velocity,0);
        } else{
            collider.attachedRigidbody.velocity=teleportMatrix.Mul(collider.attachedRigidbody.velocity,0);
        }
    }
    IEnumerator PopAfterSecs(float sec){
        yield return new WaitForSeconds(sec);
        linkedPortal.teleportedObjs.Dequeue();
    }
}