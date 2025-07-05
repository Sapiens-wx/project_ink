using System;
using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] internal Collider2D damageBox;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] internal int maxHealth;

    int curHealth;
    public int CurHealth{
        get=>curHealth;
        protected set{
            curHealth=value;
            if(curHealth<=0)
                Die();
            else if(curHealth>maxHealth) curHealth=maxHealth;
            if(healthBar!=null)
                healthBar.SetProgress((float)curHealth/maxHealth);
        }
    }
    protected int dir;
    public virtual int Dir{
        get=>dir;
        set{
            if(dir==value) return;
            dir=value;
            transform.localScale=new Vector3(dir==1?-Mathf.Abs(transform.localScale.x):Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            //spr.transform.localScale=new Vector3(dir==1?-Mathf.Abs(spr.transform.localScale.x):Mathf.Abs(spr.transform.localScale.x), spr.transform.localScale.y, spr.transform.localScale.z);
            if(healthBar!=null){ //make sure not to flip the health bar.
                Vector3 localScale=healthBar.transform.localScale;
                localScale.x=dir==1?Mathf.Abs(localScale.x):-Mathf.Abs(localScale.x);
                healthBar.transform.localScale=localScale;
            }
        }
    }
    [HideInInspector] public int id; //id in a room
    [NonSerialized] public Collider2D bc;
    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public Animator animator;
    public Coroutine jumpDownCoro;
    /// <summary>
    /// called when the enemy gets hit.
    /// </summary>
    public virtual void OnHit(HitEnemyInfo info){
        CardEventManager.onCardDealDamage?.Invoke(info);
        OnDamaged(info.damage);
    }
    public virtual void OnDamaged(int damage){
        if(CurHealth>0)
            CurHealth-=damage;
    }
    public virtual void OnHealed(int amount){
        CurHealth+=amount;
    }
    public virtual void Die(){
        if(PlanetVisualizer.inst.uranusesDict.ContainsKey(this))
            PlanetVisualizer.inst.RemoveUranus(this);
        Destroy(gameObject);
    }
    internal virtual void Start(){
        if(bc!=null) return; //it means start function is called else where. no need to initialize again
        Dir=1;
        RoomManager.inst.RegisterEnemy(this);
        CurHealth=maxHealth;
        bc=GetComponent<Collider2D>();
        rgb=GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
    }
    protected virtual void OnDestroy(){
        RoomManager.inst.UnRegisterEnemy(this);
    }
    public bool PlayerInRange(float dist){
        Vector2 dir=PlayerShootingController.inst.transform.position-transform.position;
        //not in the range
        if(!(dir.x*dir.x+dir.y*dir.y<=dist*dist))
            return false;
        //is in the range, but enemy's view is blocked by the wall
        if(Physics2D.Linecast(transform.position, PlayerCtrl.inst.transform.position, GameManager.inst.groundMixLayer))
            return false;
        return true;
    }
    public bool PlayerInRange(Bounds b){
        b.center+=transform.position;
        Vector2 min=b.min, max=b.max;
        Vector2 pos=PlayerShootingController.inst.transform.position;
        //not in the range
        if(pos.x<min.x||pos.x>max.x||pos.y<min.y||pos.y>max.y)
            return false;
        //is in the range, but enemy's view is blocked by the wall
        if(Physics2D.Linecast(transform.position, PlayerCtrl.inst.transform.position, GameManager.inst.groundMixLayer))
            return false;
        return true;
    }
    /// <summary>
    /// make the enemy stick to the ground by changing the y pos (raycast downward)
    /// </summary>
    public void ToTheGround(){
        RaycastHit2D hit=Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, GameManager.inst.groundMixLayer);
        if(hit)
            transform.position=new Vector3(transform.position.x, hit.point.y+bc.bounds.extents.y+bc.offset.y, 0);
    }
    /// <summary>
    /// make the enemy face the player
    /// </summary>
    public void UpdateDir(){
        Dir=(int)Mathf.Sign(PlayerCtrl.inst.transform.position.x-transform.position.x);
    }
    public bool DirectlyJumpDownIfCan(){
        Vector2 startPos=bc.bounds.center;
        Vector2 lineToPos=startPos-new Vector2(0, bc.bounds.extents.y+.2f);
        //use a line to detect the ground's collider that the enemy is stand on.
        RaycastHit2D hit=Physics2D.Linecast(startPos, lineToPos, GameManager.inst.platformLayer);
        if(!hit) return false;
        if(jumpDownCoro!=null) return false;
        jumpDownCoro=StartCoroutine(JumpDown(hit));
        return true;
    }
    IEnumerator JumpDown(RaycastHit2D hit){
        WaitForSeconds wait=new WaitForSeconds(.2f);
        Vector2 startPos,lineToPos;
        //ignore the collision until the enemy is completely under the platform
        Physics2D.IgnoreCollision(hit.collider, bc);
        yield return new WaitForSeconds(.5f);
        for(float startT=Time.time, t=startT;t-startT<20; t=Time.time){
            startPos=new Vector2(bc.bounds.center.x, bc.bounds.max.y);
            lineToPos=startPos-new Vector2(0, bc.bounds.size.y-.1f);
            if(!Physics2D.Linecast(startPos,lineToPos, GameManager.inst.platformLayer))
                break;
            yield return wait;
        }
        Physics2D.IgnoreCollision(hit.collider, bc, false);
        jumpDownCoro=null;
    }
}
