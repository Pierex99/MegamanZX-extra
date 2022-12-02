using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //referencias
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;

    [Header("Velocidad")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float jumpSpeed = 3.7f;

    [Header("Ataques")]
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] Transform bulletShootPos;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject sword;

    [Header("SaltoPared")]
    [SerializeField] private Transform controladorPared;
    [SerializeField] private Vector3 dimensionesCajaPared;
    private bool enPared;
    private bool deslizando;
    [SerializeField] private float velocidadDeslizar;
    [SerializeField] private LayerMask queEsSuelo;

    [SerializeField] private float fuerzaSaltoParedX;
    [SerializeField] private float fuerzaSaltoParedY;
    [SerializeField] private float tiempoSaltoPared;
    private bool saltandoDePared;

    AudioSource audio_S;
    public AudioClip[] sound;

    //ingreso de teclado
    float keyHorizontal;
    bool keyJump;
    bool keyShoot;
    bool keySword;

    //verificadores
    bool isGrounded;
    bool isShooting;
    bool isSwording;
    bool isAttacking = false;
    bool isTakingDamage;
    bool isInvincible;
    bool isFacingRight;

    bool hitSideRight;

    float shootTime;
    bool keyShootRelease;

    float swordTime;
    bool keySwordRelease;

    public int currentHealth;
    public int maxHealth = 28;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        audio_S = GetComponent<AudioSource>();

        isFacingRight = true;

        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
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

        enPared = Physics2D.OverlapBox(controladorPared.position, dimensionesCajaPared, 0f, queEsSuelo);
        if (deslizando)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -velocidadDeslizar, float.MaxValue));
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (isTakingDamage)
        {
            animator.Play("model-zx_hit");
            return;
        }
        if (isShooting || isSwording) isAttacking = true;
        else isAttacking = false;

        if (!isGrounded && enPared && keyHorizontal != 0) deslizando = true;
        else deslizando = false;

        PlayerDirectionInput();
        PlayerJumpInput();
        PlayerShootInput();
        PlayerSwordInput();
        PlayerMovement();
    }

    void PlayerDirectionInput()
    {
        keyHorizontal = Input.GetAxisRaw("Horizontal");
    }

    void PlayerJumpInput()
    {
        keyJump = Input.GetKeyDown(KeyCode.Space);
    }

    void PlayerShootInput()
    {
        float shootTimeLength;
        float keyShootReleaseTimeLength = 0;

        keyShoot = Input.GetKey(KeyCode.X);

        if (keyShoot && keyShootRelease && !isAttacking)
        {
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            // Shoot Bullet
            Invoke("ShootBullet", 0.1f);
            audio_S.clip = sound[0];
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

    void PlayerSwordInput()
    {
        float swordTimeLength;
        float keySwordReleaseTimeLength = 0;
        keySword = Input.GetKey(KeyCode.Z);

        if (keySword && keySwordRelease && !isAttacking)
        {
            isSwording = true;
            keySwordRelease = false;
            swordTime = Time.time;
            // Ataque de espada
            // que se active el game object "sword"
            //Invoke("SwordAttack", 0.1f);
            // que el circle collider busque trigger
            audio_S.clip = sound[3];
            audio_S.Play();
        }
        if (!keySword && !keySwordRelease)
        {
            keySwordReleaseTimeLength = Time.time - swordTime;
            keySwordRelease = true;
        }

        if (isSwording)
        {
            swordTimeLength = Time.time - swordTime;
            if (swordTimeLength >= 0.35f || keySwordReleaseTimeLength >= 0.15f)
            {
                isSwording = false;
            }
        }
    }

    void PlayerMovement()
    {
        if (!saltandoDePared)
        {
            if (keyHorizontal < 0)
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
                    else if (isSwording)
                    {
                        animator.Play("model-zx_sword1");
                    }
                    else
                    {
                        animator.Play("model-zx_idle");
                    }
                }
                rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
            }
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
                audio_S.clip = sound[2];
                audio_S.Play();
            }
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }

        if (!isGrounded)
        {
            if (isShooting)
            {
                animator.Play("model-zx_jumpShoot");
            }
            else if (deslizando && !keyJump)
            {
                animator.Play("model-zx_slide");

            }
            else if (deslizando && keyJump)
            {
                Debug.Log("salto paredX: " + fuerzaSaltoParedX * -keyHorizontal);
                rb2d.velocity = new Vector2(fuerzaSaltoParedX * -keyHorizontal, fuerzaSaltoParedY);
                StartCoroutine(CambioSaltoPared());
            }
            else
            {
                animator.Play("model-zx_jump");
            }
        }
    }

    IEnumerator CambioSaltoPared()
    {
        saltandoDePared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoDePared = false;
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

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
                Over.showGameOver();
            }
            else
            {
                StartDamageAnimation();
                audio_S.clip = sound[1];
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


    void StartSwordAnimation()
    {
        sword.SetActive(true);
    }
    void StopSwordAnimation()
    {
        sword.SetActive(false);
    }


    void Defeat()
    {
        Destroy(gameObject);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorPared.position, dimensionesCajaPared);
    }
}
