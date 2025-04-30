using UnityEngine;

public abstract class BossBase : EnemyBase
{
    [SerializeField] float detectDist;

    bool activated;
    float detectDistSqrd;
    public override int Dir{
        get=>dir;
        set{
            if(dir==value) return;
            dir=value;
            transform.localScale=new Vector3(dir==1?-Mathf.Abs(transform.localScale.x):Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    protected virtual void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position,detectDist);
    }
    internal override void Start()
    {
        base.Start();
        detectDistSqrd=detectDist*detectDist;
    }
    void FixedUpdate(){
        if(!activated){
            Vector2 dir=PlayerCtrl.inst.transform.position-transform.position;
            if(dir.x*dir.x+dir.y*dir.y<=detectDistSqrd){
                activated=true;
                animator.SetBool("b_detect",true);
            }
        }
    }
}