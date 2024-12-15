using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamageCtrl : Singleton<PlayerDamageCtrl>
{
    public event System.Action<Collider2D> onHitByEnemy; //invoked when colliders with the enemyBullet layer hit the player
    void OnTriggerEnter2D(Collider2D collider){
        if(GameManager.IsLayer(GameManager.inst.enemyBulletLayer, collider.gameObject.layer))
            onHitByEnemy?.Invoke(collider);
    }
}