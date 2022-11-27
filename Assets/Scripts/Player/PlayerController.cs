using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //referencias
    Animator animator;//
    BoxCollider2D box2d;
    Rigidbody2D rb2d;

    //variables movimiento
    [SerializeField] public float moveSpeed = 1.5f;
    [SerializeField] public float jumpSpeed = 3.7f;

    //variables ataque disparo
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] Transform bulletShootPos;
    [SerializeField] GameObject bulletPrefab;

    AudioSource audio_S;
    public AudioClip[] sound;

    //ingreso de teclado
    float keyHorizontal;
    bool keyJump;
    bool keyShoot;

    //verificadores
    public bool isGrounded;
    public bool isShooting;
    bool isTakingDamage;
    bool isInvincible;
    bool isFacingRight;

    bool hitSideRight;

    float shootTime;
    public bool keyShootRelease;

    //vida
    public int currentHealth;
    public int maxHealth = 28;
    
    private FiniteStateMachine<PlayerController> mFsm;

    // Estados (instancias)
    public PlayerStateIdle idleState;
    public PlayerStateJump jumpState;
    public PlayerStateRun runState;
    public PlayerStateHit hitState;
    public PlayerStateShoot shootState;
    public PlayerStateRunShoot runShootState;
    public PlayerStateJumpShoot jumpShootState;

    void Awake()
    {
        mFsm = new FiniteStateMachine<PlayerController>();
        idleState = new PlayerStateIdle(this, mFsm);
        jumpState = new PlayerStateJump(this, mFsm);
        runState = new PlayerStateRun(this, mFsm);
        hitState = new PlayerStateHit(this, mFsm);
        shootState = new PlayerStateShoot(this, mFsm);
        runShootState = new PlayerStateRunShoot(this, mFsm);
        jumpShootState = new PlayerStateJumpShoot(this, mFsm);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        audio_S = GetComponent<AudioSource>();

        isFacingRight = true;

        currentHealth = maxHealth;

        mFsm.Start(idleState);
    }

    private void FixedUpdate()
    {
        //mFsm
        mFsm.CurrentState.OnPhysicsUpdate();


        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        // ground check
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        // player box colliding with ground layer
        if (raycastHit.collider != null)
        {
            isGrounded = true;
        }
        // draw debug lines
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);


    }
    // Update is called once per frame
    void Update()
    {
        //mFsm
        mFsm.CurrentState.HandleInput();
        mFsm.CurrentState.OnLogicUpdate();


        if (isTakingDamage)
        {
            animator.Play("model-zx_hit");
            return;
        }

        /* PlayerDirectionInput();
        PlayerJumpInput(); */
        PlayerShootInput();
        //PlayerMovement();
    }

    /* void PlayerDirectionInput()
    {
        keyHorizontal = Input.GetAxisRaw("Horizontal");//
    } */

    /* void PlayerJumpInput()
    {
        keyJump = Input.GetKeyDown(KeyCode.Space);//
    } */

    void PlayerShootInput()
    {
        float shootTimeLength;
        float keyShootReleaseTimeLength = 0;

        //keyShoot = Input.GetKey(KeyCode.X);//

        if (keyShoot && keyShootRelease)
        {
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            // Shoot Bullet
            Invoke("ShootBullet", 0.1f);
            audio_S.clip = sound[3];
            audio_S.Play();
        }
        if (!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        if (isShooting)
        {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength >= 0.25f || keyShootReleaseTimeLength >= 0.15f)
            {
                isShooting = false;
            }
        }
    }

    /* void PlayerMovement()
    {
        if (keyHorizontal < 0)//
        {
            if (isFacingRight)
            {
                Flip();
            }
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("model-zx_runShoot");
                }
                else
                {
                    animator.Play("model-zx_run");
                }
            }
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
        }
        else if (keyHorizontal > 0)
        {
            if (!isFacingRight)
            {
                Flip();
            }
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("model-zx_runShoot");
                }
                else
                {
                    animator.Play("model-zx_run");
                }
            }
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
        }
        else
        {
            if (isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("model-zx_shoot");
                }
                else
                {
                    animator.Play("model-zx_idle");
                }
            }
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }

        if (keyJump && isGrounded)
        {
            if (isShooting)
            {
                animator.Play("model-zx_jumpShoot");
            }
            else
            {
                animator.Play("model-zx_jump");
            }
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }

        if (!isGrounded)
        {
            if (isShooting)
            {
                animator.Play("model-zx_jumpShoot");
            }
            else
            {
                animator.Play("model-zx_jump");
            }
        }
    } */
    /* void Flip()//
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    } */

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletShootPos.position, Quaternion.identity);
        bullet.name = bulletPrefab.name;
        bullet.GetComponent<BulletScript>().SetDamageValue(bulletDamage);
        bullet.GetComponent<BulletScript>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<BulletScript>().SetBulletDirection((isFacingRight) ? Vector2.right : Vector2.left);
        bullet.GetComponent<BulletScript>().Shoot();
    }

    public void HitSide(bool rightSide)
    {
        hitSideRight = rightSide;
    }

    public void Invincible(bool invincibility)
    {
        isInvincible = invincibility;
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            Mathf.Clamp(currentHealth, 0, maxHealth);
            UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
            if (currentHealth <= 0)
            {
                Defeat();
            }
            else
            {
                StartDamageAnimation();
                audio_S.clip = sound[4];
                audio_S.Play();
            }
        }
    }

    void StartDamageAnimation()
    {
        if (!isTakingDamage)
        {
            isTakingDamage = true;
            isInvincible = true;
            float hitForceX = 0.90f;
            float hitForceY = 1.5F;
            if (hitSideRight) hitForceX = -hitForceX;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(hitForceX, hitForceY), ForceMode2D.Impulse);
        }
    }

    void StopDamageAnimation()
    {
        isTakingDamage = false;
        isInvincible = false;
        animator.Play("model-zx_hit", -1, 0f);
    }


    void Defeat()
    {
        Destroy(gameObject);
    }

}