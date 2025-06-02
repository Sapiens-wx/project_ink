using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Cannon : TrapBase
{
    [SerializeField] float shootInterval;

    Coroutine coro;
    void OnEnable(){
        coro=StartCoroutine(Shoot());
    }
    IEnumerator Shoot(){
        WaitForSeconds wait=new WaitForSeconds(shootInterval);
        while(EnemyBulletManager.inst==null)
            yield return null;
        float rangexmin=RoomManager.CurrentRoom.RoomBounds.min.x, rangexmax=RoomManager.inst.RoomBounds.max.x;
        float rangexInterval=(rangexmax-rangexmin)/3;
        while(true){
            int[] xindices=MathUtil.GetRandIndices(3);
            for(int i=0;i<3;++i){
                int xindex=xindices[i];
                float xrangeStart=rangexmin+rangexInterval*xindex;
                //intantiate bullet
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet(EnemyBulletManager.inst.trap_cannon[(int)theme]);
                bullet.transform.position=transform.position;
                //x position of the bullet when hit the ground
                float g=bullet.rgb.gravityScale*9.8f, y1=RoomManager.inst.RoomBounds.min.y;
                float vy=Mathf.Sqrt(2*g*(RoomManager.CurrentRoom.RoomBounds.max.y-transform.position.y)); //vy=2g*height
                float randBulletX=Random.Range(xrangeStart, xrangeStart+rangexInterval);
                float distx=randBulletX-bullet.transform.position.x;
                float t1=vy/g;
                float h0=vy*t1-0.5f*g*t1*t1;
                float h=bullet.transform.position.y+h0-y1;
                float t2=Mathf.Sqrt(2*h/g);
                float t=t1+t2;
                float vx=distx/t;
                bullet.rgb.velocity=new Vector2(vx,vy);
                yield return wait;
            }
            yield return 0;
        }
    }
}
