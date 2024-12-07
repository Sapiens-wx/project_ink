using System.Collections;
using UnityEngine;

public class EB_Monkey : EnemyBulletBase {
    [SerializeField] float splitDist;
    internal override void Start(){
        base.Start();
        StartCoroutine(SplitAfterDist());
    }
    IEnumerator SplitAfterDist(){
        yield return new WaitForSeconds(splitDist/spd);
        Vector2 dir=rgb.velocity.normalized;
        float cos=1.732051f, sin=.5f; //cos(PI/6) and sin(PI/6). 30 degree
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.monkey_split, transform.position, new Vector2(dir.x*cos-dir.y*sin, dir.x*sin+dir.y*cos));
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.monkey_split, transform.position, new Vector2(dir.x*cos+dir.y*sin, -dir.x*sin+dir.y*cos));
        Destroy(gameObject);
    }
}