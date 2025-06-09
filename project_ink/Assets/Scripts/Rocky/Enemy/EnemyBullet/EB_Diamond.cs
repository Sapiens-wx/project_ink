using UnityEngine;

public class EB_Diamond:EnemyBulletBase{
    protected override void FixedUpdate(){
        base.FixedUpdate();
        float angle=Vector2.SignedAngle(Vector2.right, velocity);
        transform.eulerAngles=new Vector3(0,0,angle);
    }
}