using System.Collections;
using UnityEngine;

public class EB_Elf : EnemyBulletBase {
    internal override void Start(){
        base.Start();
    }
    internal override void OnTriggerEnter2D(Collider2D collider){
        float cos=Mathf.Cos(Mathf.PI/6), sin=Mathf.Sin(Mathf.PI/6); //cos(PI/6) and sin(PI/6). 30 degree
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.elf_split, transform.position+new Vector3(0,.15f,0), new Vector2(-sin, cos));
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.elf_split, transform.position+new Vector3(0,.15f,0), new Vector2(+sin, cos));
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.elf_split, transform.position+new Vector3(0,.15f,0), Vector2.up);
        base.OnTriggerEnter2D(collider);
    }
}