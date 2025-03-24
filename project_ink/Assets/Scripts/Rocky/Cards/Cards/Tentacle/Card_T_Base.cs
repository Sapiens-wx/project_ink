using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card_T_Base : Card
{
    public override Projectile FireCard(Projectile.ProjectileType type, bool returnToCardPool){
        if(damage>0){
            switch(type){
                case Projectile.ProjectileType.AutoChase:
                case Projectile.ProjectileType.AutoFire:
                    EnemyBase enemy=RoomManager.CurrentRoom.ClosestEnemy(PlayerCtrl.inst.transform);
                    if(enemy!=null)
                        TentacleManager.inst.tentacle.Attack(enemy.transform.position, damage);
                    break;
                case Projectile.ProjectileType.Normal:
                    TentacleManager.inst.tentacle.Attack(PlayerCtrl.inst.mouseWorldPos, damage);
                    break;
            }
        }
        if(returnToCardPool)
            ReturnToCardPool();
        return null;
    }
}