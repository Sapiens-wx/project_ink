using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Animator swordAnimator;
    public BoxCollider2D bc;
    public SpriteRenderer spr;
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
    public float invincibleTime;
    public float hitAnimDuration, counterAnimDuration;
    public float timeStopInterval;

    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public Animator animator;

    //inputs
    [HideInInspector] public int inputx;

    [HideInInspector] public static PlayerCtrl inst;
    [HideInInspector] public Vector2 v; //velocity
    [HideInInspector] public bool hittable;
    [HideInInspector] public bool onGround, prevOnGround;
    [HideInInspector] public float jumpKeyDown;
    [HideInInspector] public bool jumpKeyUp;
    [HideInInspector] public float onGroundTime; //onGroundTime is used for coyote time
    [HideInInspector] public bool dashing, canDash;
    [HideInInspector] public float dashKeyDown, allowDashTime;
    [HideInInspector] public int dashDir;
    [HideInInspector] public float yspd;
    [HideInInspector] public int dir;
    [HideInInspector] public MaterialPropertyBlock matPB;
    [HideInInspector] public Sequence hitAnim, invincibleAnim;
    [HideInInspector] public Collider2D hitBy;
    [HideInInspector] public Vector2 mouseWorldPos;
    [HideInInspector] public Camera mainCam;
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
    public event Action<Collider2D> onPlayerHit;
    public int Dir{
        get=>dir;
        set{
            if(dir==value) return;
            dir=value;
            leftTop.x*=-1;
            rightTop.x*=-1;
            leftBot.x*=-1;
            rightBot.x*=-1;
            transform.localScale=new Vector3(dir,1,1);
            return;
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
        readInput=true;
        hittable=true;
        Dir=-1;
        jumpKeyDown=-100;
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
            else if(Input.GetKeyDown(KeyCode.W)){ //jump
                jumpKeyDown=Time.time;
            }
            else if(Input.GetKeyUp(KeyCode.W)){ //jump key up
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
        Collider2D hit = Physics2D.OverlapArea((Vector2)transform.position+leftBot, (Vector2)transform.position+rightBot, GameManager.inst.groundLayer);
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
    void OnTriggerStay2D(Collider2D collider){
        //if is enemy, player is hit
        if(hittable && GameManager.IsLayer(GameManager.inst.enemyBulletLayer, collider.gameObject.layer)){ 
            PlayerCtrl.inst.animator.SetTrigger("hit");
            hitBy=collider;
            onPlayerHit?.Invoke(collider);
        }
    }
}
