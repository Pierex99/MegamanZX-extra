
using UnityEngine;

public class PlayerStateJump : State<PlayerController>
{
    private Rigidbody2D rb2d;
    private Animator animator;
    public PlayerStateJump(
        PlayerController controller, 
        FiniteStateMachine<PlayerController> fsm
    ) : base(controller, fsm)
    {
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void OnLogicUpdate()
    {
        base.OnLogicUpdate();
    }

    public override void OnPhysicsUpdate()
    {
        base.OnPhysicsUpdate();
    }

    public override void OnStart()
    {
        base.OnStart();
        rb2d = mController.transform.GetComponent<Rigidbody2D>();
        animator = mController.transform.GetComponent<Animator>();

        rb2d.velocity = new Vector2(rb2d.velocity.x, mController.jumpSpeed);
        animator.Play("model-zx_jump");
    }

    public override void OnStop()
    {
        base.OnStop();
    }
}
