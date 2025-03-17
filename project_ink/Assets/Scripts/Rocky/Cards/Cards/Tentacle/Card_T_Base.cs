using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card_T_Base : Card
{
    public override Projectile FireCard(Projectile.ProjectileType type, bool returnToCardPool){
        switch(type){
            case Projectile.ProjectileType.AutoChase:
            case Projectile.ProjectileType.AutoFire:
                EnemyBase enemy=RoomManager.CurrentRoom.ClosestEnemy(PlayerCtrl.inst.transform);
                if(enemy!=null)
                    Tentacle.inst.Attack(enemy.transform.position);
                break;
            case Projectile.ProjectileType.Normal:
                Tentacle.inst.Attack(PlayerCtrl.inst.mouseWorldPos);
                break;
        }
        if(returnToCardPool)
            ReturnToCardPool();
        return null;
    }
}