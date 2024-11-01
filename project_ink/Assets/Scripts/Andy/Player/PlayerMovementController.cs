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
    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    public float runDecceleration;
    public float accelInAir;
    public float deccelInAir;
    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
    public float jumpHangTimeThreshold;
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;
    private float jumpForce;
    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public float dashCoolDown;
    [Header("Gravity")]
    public float jumpCutGravityMult;
    public float maxFallSpeed;
    [Range(0f, 1)] public float jumpHangGravityMult;
    public float fallGravityMult;
    private float gravityStrength;
    private float gravityScale;
    //--public float invincibilityTime;

    //checks
    [SerializeField] private Transform _groundCheckPoint;
        //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    //layers
    [SerializeField] private LayerMask _groundLayer;

    //platforms
    private GameObject currentPlatformCollider;

    //booleans
    public bool isGround;

    private bool isJumping;
    private bool isDashing = false;
    private bool canDash = true;
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private bool isFacingRight;
    private bool doConserveMomentum;

    //input
    private Vector2 _moveInput;

    //timers
    private float LastOnGroundTime;
    private float LastPressedJumpTime;

    //store
    private float runAccelAmount;
    private float runDeccelAmount;

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
        isFacingRight = true;
        CurrentFacingDirection = FACING_DIRECTION.RIGHT;
    }
    
    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
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
        LastOnGroundTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;

        //get down platform
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentPlatformCollider != null)
                {
                    StartCoroutine(DisablePlatformCoroutine());
                }
            }
        }

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        Run(1);

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown("w") || Input.GetKeyDown("up")) && !Input.GetKey(KeyCode.S))
        {
            OnJumpInput();
        }
        
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown("w") || Input.GetKeyDown("up")) && !Input.GetKey(KeyCode.S))
        {
            OnJumpUpInput();
        }
        #endregion
        if (!isJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !isJumping)
            {
                LastOnGroundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
            }
        }
        if (isJumping && myRigidbody.velocity.y < 0)
        {
            isJumping = false;
        }

        if (LastOnGroundTime > 0 && !isJumping)
        {
            _isJumpCut = false;

            if (!isJumping)
                _isJumpFalling = false;
        }

        //Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            isJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        if (_isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(gravityScale * jumpCutGravityMult);
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Max(myRigidbody.velocity.y, -maxFallSpeed));
        }
        else if ((isJumping || _isJumpFalling) && Mathf.Abs(myRigidbody.velocity.y) < jumpHangTimeThreshold)
        {
            SetGravityScale(gravityScale * jumpHangGravityMult);
        }
        else if (myRigidbody.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(gravityScale * fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, Mathf.Max(myRigidbody.velocity.y, -maxFallSpeed));
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(gravityScale);
        }

        CheckGrounded();
        SwitchAnimation();
    }

    void CheckGrounded()
    {
        isGround = myFeet.IsTouchingLayers(_groundLayer);
    }

    public void OnJumpInput()
    {
        LastPressedJumpTime = jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }
    public void SetGravityScale(float scale)
    {
        myRigidbody.gravityScale = scale;
    }

    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(myRigidbody.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((isJumping || _isJumpFalling) && Mathf.Abs(myRigidbody.velocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (doConserveMomentum && Mathf.Abs(myRigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(myRigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - myRigidbody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        myRigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = jumpForce;
        if (myRigidbody.velocity.y < 0)
            force -= myRigidbody.velocity.y;

        myRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        return isJumping && myRigidbody.velocity.y > 0;
    }
    #endregion

    //void Flip()
    //{
    //    bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
    //
    //    if (playerHasXAxisSpeed)
    //    {
    //        if (myRigidbody.velocity.x > 0.1f)
    //        {
    //            transform.localRotation = Quaternion.Euler(0, 0, 0);
    //            CurrentFacingDirection = FACING_DIRECTION.RIGHT;
    //        }
    //
    //        if (myRigidbody.velocity.x < -0.1f)
    //        {
    //            transform.localRotation = Quaternion.Euler(0, 180, 0);
    //            CurrentFacingDirection = FACING_DIRECTION.LEFT;
    //        }
    //    }
    //}

    //void Running()
    //{
    //    float moveDir = Input.GetAxis("Horizontal");
    //    Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
    //    myRigidbody.velocity = playerVel;
    //    bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
    //    myAnim.SetBool("Running", playerHasXAxisSpeed);
    //    if (Input.GetKeyDown(KeyCode.LeftShift)&&canDash)
    //    {
    //        StartCoroutine(Dash());
    //    }
    //}

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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            currentPlatformCollider = other.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            currentPlatformCollider = null;
        }
    }

    private IEnumerator DisablePlatformCoroutine()
    {
        BoxCollider2D platformCollider = currentPlatformCollider.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(myFeet, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(myFeet, platformCollider, false);
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