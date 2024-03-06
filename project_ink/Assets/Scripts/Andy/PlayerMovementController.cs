using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float runSpeed;
    public float jumpSpeed;
    //--public float invincibilityTime;

    //health
    //--public int playerCurrentHealth;
    //--public PlayerHealthBarController healthBar;

    private Rigidbody2D myRigidbody;
    //private Animator myAnim;
    private BoxCollider2D myFeet;
    private bool isGround;
    //--private bool isInvincible = false;
    //--private float invincibilityTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        //myAnim = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
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
            }

            if (myRigidbody.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void Running()
    {
        float moveDir = Input.GetAxis("Horizontal");
        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        //myAnim.SetBool("Running", playerHasXAxisSpeed);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown("w") || Input.GetKeyDown("up"))
        {
            if (isGround)
            {
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigidbody.velocity = Vector2.up * jumpVel;
            }
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
