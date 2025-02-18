using Unity.VisualScripting;
using UnityEngine;

public class E_Goat_Ground : EnemyBase_Ground{
    [Header("Attack")]
    public E_Balloon balloon;
    public float flyHeight, flySpd;
    public float airDashSpd, groundDashSpd;

    [HideInInspector] public bool isDashing; //is the enemy dashing towards the player?
    internal override void Start()
    {
        base.Start();
        balloon.gameObject.SetActive(false);
        PlayerDamageCtrl.inst.onHitByEnemy+=(collider)=>{
            if(collider==damageBox){
                animator.SetTrigger("attack_hit_player");
            }
        };
        balloon.onDead+=()=>{
            rgb.gravityScale=1;
            animator.SetBool("b_lose_balloon", true);
        };
    }
    void OnCollisionEnter2D(Collision2D collision){
        if(isDashing && GameManager.IsLayer(GameManager.inst.groundLayer, collision.collider.gameObject.layer)){
            animator.SetTrigger("attack_hit_wall");
            rgb.velocity=Vector2.zero;
        }
    }
}