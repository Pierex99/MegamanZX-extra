
using UnityEngine;

public class PlayerStateRun : State<PlayerController>
{
    private float keyHorizontal;
    private bool keyJump = false;
    private bool keyShoot = false;

    bool isFacingRight = true;

    private Animator animator;
    private Rigidbody2D rb2d;

    public PlayerStateRun(
        PlayerController controller, 
        FiniteStateMachine<PlayerController> fsm) : base(controller, fsm)
    {
    }

    public override void HandleInput()
    {
        base.HandleInput();
        keyHorizontal = Input.GetAxisRaw("Horizontal");
        keyJump = Input.GetKeyDown(KeyCode.Space);
        keyShoot = Input.GetKey(KeyCode.X);
    }

    public override void OnLogicUpdate()
    {
        base.OnLogicUpdate();
        if (keyHorizontal == 0f) mFsm.ChangeState(mController.idleState);
        if (keyJump && mController.isGrounded) mFsm.ChangeState(mController.jumpState);
        if (keyShoot && mController.keyShootRelease) mFsm.ChangeState(mController.shootState);

        if (!mController.isGrounded && mController.isShooting) mFsm.ChangeState(mController.jumpShootState);

        mFsm.ChangeState(mController.runShootState);
        mFsm.ChangeState(mController.hitState);

        if (keyHorizontal < 0)
        {
            if (mController.isGrounded)
            {
                if (mController.isShooting)
                {
                    animator.Play("model-zx_runShoot");
                }
                else
                {
                    animator.Play("model-zx_run");
                }
            }
            
        }
        else if (keyHorizontal > 0)
        {
            if (mController.isGrounded)
            {
                if (mController.isShooting)
                {
                    animator.Play("model-zx_runShoot");
                }
                else
                {
                    animator.Play("model-zx_run");
                }
            }
            
        }
        else //idle => shoot
        {
            if (mController.isGrounded)
            {
                if (mController.isShooting)
                {
                    animator.Play("model-zx_shoot");
                }
                else
                {
                    animator.Play("model-zx_idle");
                }
            }
            
        }
    }

    public override void OnPhysicsUpdate()
    {
        base.OnPhysicsUpdate();
        if (keyHorizontal < 0)
        {
            if (isFacingRight)
            {
                Flip();
            }
            rb2d.velocity = new Vector2(-mController.moveSpeed, rb2d.velocity.y);
        }
        else if (keyHorizontal > 0)
        {
            if (!isFacingRight)
            {
                Flip();
            }
            rb2d.velocity = new Vector2(mController.moveSpeed, rb2d.velocity.y);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        animator = mController.transform.GetComponent<Animator>();
        rb2d = mController.transform.GetComponent<Rigidbody2D>();
    }

    public override void OnStop()
    {
        base.OnStop();
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        mController.transform.Rotate(0f, 180f, 0f);
    }
}
