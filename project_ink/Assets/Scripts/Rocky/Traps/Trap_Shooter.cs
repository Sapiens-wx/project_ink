using UnityEngine;
using System.Collections;

public class Trap_Shooter : TrapBase{
    [SerializeField] Vector2 shootDir;
    [SerializeField] float shootInterval;

    Coroutine shootCoro;
    void OnEnable(){
        shootCoro=StartCoroutine(Shoot());
    }
    void OnDisable(){
        if(shootCoro!=null)
            StopCoroutine(shootCoro);
        shootCoro=null;
    }
    IEnumerator Shoot(){
        WaitForSeconds wait=new WaitForSeconds(shootInterval);
        while(EnemyBulletManager.inst==null)
            yield return null;
        while(true){
            EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.trap_shooter, transform.position, shootDir);
            yield return wait;
        }
    }
}