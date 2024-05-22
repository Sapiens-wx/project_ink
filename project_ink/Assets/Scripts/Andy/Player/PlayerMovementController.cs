using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FACING_DIRECTION
    {
        LEFT,
        RIGHT,
    }
    public static FACING_DIRECTION CurrentFacingDirection;
    //adjustables
    public float runSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float dashTime;
    public float dashCoolDown;
    //--public float invincibilityTime;

    //booleans
    public bool isGround;

    private bool isDashing = false;
    private bool canDash = true;

    //health
    //--public int playerCurrentHealth;
    //--public PlayerHealthBarController healthBar;

    private Rigidbody2D myRigidbody;
    private Animator myAnim;
    private BoxCollider2D myFeet;
    //--private bool isInvincible = false;
    //--private float invincibilityTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
        CurrentFacingDirection = FACING_DIRECTION.RIGHT;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        //if (isInvincible)
        //{
        //    invincibilityTimer -= Time.deltaTime;
        //
        //    // Check if invincibility time has ended
        //    if (invincibilityTimer <= 0.0f)
        //    {
        //        isInvincible = false;
        //    }
        //}
        Flip();
        Running();
        Jump();
        CheckGrounded();
        SwitchAnimation();
    }

    void CheckGrounded()
    {
        isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }


    void Flip()
    {
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasXAxisSpeed)
        {
            if (myRigidbody.velocity.x > 0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                CurrentFacingDirection = FACING_DIRECTION.RIGHT;
            }

            if (myRigidbody.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                CurrentFacingDirection = FACING_DIRECTION.LEFT;
            }
        }
    }

    void Running()
    {
        float moveDir = Input.GetAxis("Horizontal");
        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("Running", playerHasXAxisSpeed);
        if (Input.GetKeyDown(KeyCode.LeftShift)&&canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void Jump()
    {
        //float currentVelX = myRigidbody.velocity.x;
        //myRigidbody.velocity = new Vector2(currentVelX, -2f);
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown("w") || Input.GetKeyDown("up"))
        {
            if (isGround)
            {
                myAnim.SetBool("Jump", true);
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigidbody.velocity = Vector2.up * jumpVel;
            }
        }
    }

    private IEnumerator Dash()
    {
        myAnim.SetBool("Dash", true);
        canDash = false;
        isDashing = true;
        float originalGravity = myRigidbody.gravityScale;
        myRigidbody.gravityScale = 0;
        if (transform.localRotation == Quaternion.Euler(0, 0, 0))
        {
            myRigidbody.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        }
        else
        {
            myRigidbody.velocity = new Vector2(transform.localScale.x * -dashSpeed, 0f);
        }
        yield return new WaitForSeconds(dashTime);
        myRigidbody.gravityScale = originalGravity;
        isDashing = false;
        myAnim.SetBool("Dash", false);
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
        
    }

    void SwitchAnimation()
    {
        if (myRigidbody.velocity.y < 0.0f)
        {
            myAnim.SetBool("Jump", false);
            myAnim.SetBool("Fall", true);
        }
        else if (isGround)
        {
            myAnim.SetBool("Fall", false);
            myAnim.SetBool("Idle", true);
        }
    }
    //public void TakeDamage(int damage)
    //{
    //    if (!isInvincible)
    //    {
    //        playerCurrentHealth -= damage;
    //        healthBar.Change(-damage);
    //
    //        // Apply invincibility
    //        isInvincible = true;
    //        invincibilityTimer = invincibilityTime;
    //
    //        if (playerCurrentHealth <= 0)
    //        {
    //            // Player dies
    //            // Destroy(gameObject);
    //        }
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.tag == "Enemy")
    //    {
    //        TakeDamage(collision.GetComponent<EnemyBase>().enemyDamage);
    //    }
    //}
}
