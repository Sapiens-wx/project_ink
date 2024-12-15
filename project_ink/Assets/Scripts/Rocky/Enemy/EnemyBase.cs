using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public SpriteRenderer spr;
    [SerializeField] internal Collider2D damageBox;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] internal int maxHealth;

    int curHealth;
    internal int CurHealth{
        get=>curHealth;
        set{
            curHealth=value;
            if(curHealth<0) curHealth=0;
            else if(curHealth>maxHealth) curHealth=maxHealth;
            if(healthBar!=null)
                healthBar.SetProgress((float)curHealth/maxHealth);
        }
    }
    int dir;
    public int Dir{
        get=>dir;
        set{
            dir=value;
            spr.transform.localScale=new Vector3(dir==1?Mathf.Abs(spr.transform.localScale.x):-Mathf.Abs(spr.transform.localScale.x), spr.transform.localScale.y, spr.transform.localScale.z);
            if(healthBar!=null){ //make sure not to flip the health bar.
                Vector3 localScale=healthBar.transform.localScale;
                localScale.x=dir==1?Mathf.Abs(localScale.x):-Mathf.Abs(localScale.x);
                healthBar.transform.localScale=localScale;
            }
        }
    }
    [HideInInspector] public int id; //id in a room
    [HideInInspector] public Collider2D bc;
    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public Animator animator;
    /// <summary>
    /// called when the enemy gets hit.
    /// </summary>
    public virtual void OnHit(Projectile proj){
        CurHealth-=proj.damage;
    }
    internal virtual void Start(){
        Dir=1;
        RoomManager.inst.RegisterEnemy(this);
        CurHealth=maxHealth;
        bc=GetComponent<Collider2D>();
        rgb=GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
    }
    void OnDestroy(){
        RoomManager.inst.UnRegisterEnemy(this);
    }
    public bool PlayerInRange(float dist){
        Vector2 dir=PlayerShootingController.inst.transform.position-transform.position;
        return dir.x*dir.x+dir.y*dir.y<=dist*dist;
    }
    public bool PlayerInRange(Bounds b){
        b.center+=transform.position;
        Vector2 min=b.min, max=b.max;
        Vector2 pos=PlayerShootingController.inst.transform.position;
        return !(pos.x<min.x||pos.x>max.x||pos.y<min.y||pos.y>max.y);
    }
}
