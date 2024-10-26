using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] GameObject prefab;
    public float projectileSpeed;


    ObjectPool<Projectile> pool;
    public static ProjectileManager inst;
    void Awake(){
        inst=this;
        pool=new ObjectPool<Projectile>(CreateFunc,OnGet, OnRelease, OnPoolObjDestroy);
    }
    Projectile CreateFunc(){
        Projectile go = Instantiate(prefab).GetComponent<Projectile>();
        go.gameObject.SetActive(false);
        return go;
    }
    static void OnGet(Projectile go){
        go.gameObject.SetActive(true);
    }
    static void OnRelease(Projectile go){
        go.gameObject.SetActive(false);
    }
    static void OnPoolObjDestroy(Projectile go){
        Destroy(go.gameObject);
    }

    public Projectile CreateProjectile(){
        return pool.Get();
    }
    public void ReleaseProjectile(Projectile go){
        pool.Release(go);
    }
}
