using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public BoxCollider2D bc;
    public SpriteRenderer spr;
    public Tentacle tentacle;
    public float gravity, maxFallSpd;
    public float keyDownBuffTime;
    [Header("Movement")]
    public float xspd;
    [Header("Jump")]
    public KeyCode jumpKey;
    public float jumpHeight;
    public float jumpInterval;
    public float jumpBufferTime, coyoteTime;
    [Header("Dash")]
    public float dashDist;
    public float[] dashPercents;
    public float dashInterval;
    [Header("Ground Check")]
    public Vector2 leftBot;
    public Vector2 rightBot;
    [Header("Ceiling Check")]
    public Vector2 leftTop;
    public Vector2 rightTop;
    [Header("Hit")]
    [SerializeField] ProgressBar healthBar;
    [SerializeField] int maxHealth;
    public float invincibleTime;
    public float hitAnimDuration, counterAnimDuration;
    public float timeStopInterval;

    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public Animator animator;

    //inputs
    [NonSerialized][HideInInspector] public int inputx;

    [HideInInspector] public static PlayerCtrl inst;
    private int hp;
    [NonSerialized][HideInInspector] public Vector2 v; //velocity
    [NonSerialized][HideInInspector] public bool hittable;
    [NonSerialized][HideInInspector] public bool onGround, prevOnGround;
    [NonSerialized][HideInInspector] public float jumpKeyDown, lastJumpKeyDown, secondToLastJumpKeyDown; //jumpKeyDown will be set to -100 when detected. lastJumpKeyDown stores the last JumpKeyDown time. (before setting jumpKeyDown to -100, jumpKeyDown=lastJumpKeyDown)
    [NonSerialized][HideInInspector] public bool jumpKeyUp;
    [NonSerialized][HideInInspector] public float onGroundTime; //onGroundTime is used for coyote time
    [NonSerialized][HideInInspector] public bool dashing, canDash;
    [NonSerialized][HideInInspector] public float dashKeyDown, allowDashTime;
    [NonSerialized][HideInInspector] public int dashDir;
    [NonSerialized][HideInInspector] public float yspd;
    [NonSerialized][HideInInspector] public int dir;
    [NonSerialized][HideInInspector] public MaterialPropertyBlock matPB;
    [NonSerialized][HideInInspector] public Sequence hitAnim, invincibleAnim;
    [NonSerialized][HideInInspector] public Collider2D hitBy;
    [NonSerialized][HideInInspector] public Vector2 mouseWorldPos;
    [NonSerialized][HideInInspector] public Camera mainCam;
    //ignore collision
    [NonSerialized] public List<Collider2D> ignoredColliders;
    //read input
    bool readInput;
    //movable platform
    public bool ReadInput{
        get=>readInput;
        set{
            readInput=value;
            if(!readInput){
                inputx=0;
            }
        }
    }
    public int Health{
        get=>hp;
        set{
            hp=value;
            if(hp<0) hp=0;
            healthBar.SetProgress((float)hp/maxHealth);
        }
    }
    public event Action<Collider2D> onPlayerHit;
    public int Dir{
        get=>dir;
        set{
            if(dir==value) return;
            //not flip the tentacle
            tentacle.Dir=-tentacle.Dir;
            //
            dir=value;
            leftTop.x*=-1;
            rightTop.x*=-1;
            leftBot.x*=-1;
            rightBot.x*=-1;
            transform.localScale=new Vector3(dir,1,1);
            //not to flip the healthBar
            Vector3 localScale=healthBar.transform.localScale;
            localScale.x=dir==1?Mathf.Abs(localScale.x):-Mathf.Abs(localScale.x);
            healthBar.transform.localScale=localScale;
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color=Color.green;
        //ground check
        Gizmos.DrawLine((Vector2)transform.position+leftBot, (Vector2)transform.position+rightBot);
        //ceiling check
        Gizmos.DrawLine((Vector2)transform.position+leftTop, (Vector2)transform.position+rightTop);
        //jump height
        Gizmos.DrawLine(transform.position+new Vector3(-.2f,0,0),transform.position+new Vector3(.2f,0,0));
        Gizmos.DrawLine(transform.position,transform.position+new Vector3(0,jumpHeight,0));
        Gizmos.DrawLine(transform.position+new Vector3(-.2f,jumpHeight,0),transform.position+new Vector3(.2f,jumpHeight,0));
    }
    void Awake(){
        inst=this;
        mainCam=Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        ignoredColliders=new List<Collider2D>();

        readInput=true;
        hittable=true;
        Health=maxHealth;
        Dir=-1;
        jumpKeyDown=-100;
        lastJumpKeyDown=-100;
        secondToLastJumpKeyDown=-100;
        dashKeyDown=-100;

        allowDashTime=0;

        rgb=GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
        yspd=jumpHeight/jumpInterval-0.5f*gravity*jumpInterval;

        //hit animation
        matPB=new MaterialPropertyBlock();
        spr.GetPropertyBlock(matPB);
        matPB.SetFloat("_whiteAmount", .5f);
        spr.SetPropertyBlock(matPB);

        //invincible anim
        invincibleAnim=DOTween.Sequence();
        invincibleAnim.SetAutoKill(false);
        invincibleAnim.AppendCallback(()=>{
            hittable=false;
        });
        invincibleAnim.Append(DOTween.To(()=>matPB.GetFloat("_whiteAmount"), (val)=>{
            spr.GetPropertyBlock(matPB);
            matPB.SetFloat("_whiteAmount",val);
            spr.SetPropertyBlock(matPB);
        }, 0, hitAnimDuration).SetLoops(Mathf.RoundToInt(invincibleTime/hitAnimDuration), LoopType.Yoyo).SetEase(Ease.InOutQuad));
        invincibleAnim.AppendCallback(()=>{
            hittable=true;
            spr.GetPropertyBlock(matPB);
            matPB.SetFloat("_whiteAmount",.5f);
            spr.SetPropertyBlock(matPB);
        });
        invincibleAnim.Pause();

        hitAnim=DOTween.Sequence();
        hitAnim.SetAutoKill(false);
        hitAnim.AppendCallback(()=>{
            spr.GetPropertyBlock(matPB);
            matPB.SetFloat("_whiteAmount",0);
            spr.SetPropertyBlock(matPB);
            hittable=false;
        });
        //hitAnim.AppendCallback(()=>StartCoroutine(PauseForSeconds(.5f)));
        hitAnim.AppendInterval(0.01f);
        hitAnim.Append(DOTween.To(()=>matPB.GetFloat("_whiteAmount"), (val)=>{
            spr.GetPropertyBlock(matPB);
            matPB.SetFloat("_whiteAmount",val);
            spr.SetPropertyBlock(matPB);
        }, .5f, counterAnimDuration));
        hitAnim.AppendCallback(()=>invincibleAnim.Restart());
        hitAnim.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if(readInput){
            if(Input.GetKeyDown(KeyCode.LeftShift)){//dash
                dashKeyDown=Time.time;
            }
            else if(Input.GetKeyDown(KeyCode.Space)){ //jump
                secondToLastJumpKeyDown=lastJumpKeyDown;
                jumpKeyDown=Time.time;
                lastJumpKeyDown=jumpKeyDown;
            }
            else if(Input.GetKeyUp(KeyCode.Space)){ //jump key up
                jumpKeyUp=true;
            }
        }
    }
    void FixedUpdate(){
        UpdateMousePos();
        HandleInputs();
        CheckOnGround();
        UpdateVelocity();
    }
    void HandleInputs(){
        if(readInput)
            inputx=(int)Input.GetAxisRaw("Horizontal");
    }
    void UpdateVelocity(){
        rgb.velocity=v;
    }
    void CheckOnGround(){
        prevOnGround=onGround;
        Collider2D hit = Physics2D.OverlapArea((Vector2)transform.position+leftBot, (Vector2)transform.position+rightBot, GameManager.inst.groundMixLayer);
        foreach(Collider2D cd in ignoredColliders){
            if(hit==cd){
                hit=null;
                break;
        }}
        onGround=hit;
        if(onGround){
            canDash=true;
            onGroundTime=Time.time;
        }
    }
    void UpdateMousePos(){
        mouseWorldPos=mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
    public void UpdateDir(){
        Dir=mouseWorldPos.x>transform.position.x?-1:1;
    }
    IEnumerator PauseForSeconds(float sec){
        Time.timeScale=0;
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale=1;
    }
    void OnPlayerHit(Collider2D collider){
        EnemyBulletBase bullet=collider.GetComponent<EnemyBulletBase>();
        if(bullet!=null){
            Health-=bullet.damage;
        }
    }
    void OnTriggerStay2D(Collider2D collider){
        //if is enemy, player is hit
        if(hittable && GameManager.IsLayer(GameManager.inst.enemyBulletLayer, collider.gameObject.layer)){ 
            PlayerCtrl.inst.animator.SetTrigger("hit");
            hitBy=collider;
            onPlayerHit?.Invoke(collider);
            OnPlayerHit(collider);
        }
    }
    /// <summary>
    /// ignore the collision between 'collider' and the player collider
    /// </summary>
    public void IgnoreCollision(Collider2D collider){
        ignoredColliders.Add(collider);
        Physics2D.IgnoreCollision(bc, collider);
    }
    /// <summary>
    /// cancel all ignores of collision between the player and the colliders in 'ignoredColliders'.
    /// </summary>
    public void ClearIgnoredCollision(){
        foreach(Collider2D cd in ignoredColliders)
            Physics2D.IgnoreCollision(bc, cd);
        ignoredColliders.Clear();
    }
}
