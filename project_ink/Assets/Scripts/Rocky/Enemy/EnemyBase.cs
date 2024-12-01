using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public SpriteRenderer spr;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] internal int maxHealth;
    int curHealth;
    internal int CurHealth{
        get=>curHealth;
        set{
            curHealth=value;
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
        }
    }
    [HideInInspector] public int id; //id in a room
    [HideInInspector] public Collider2D bc;
    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public Animator animator;
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
}
