
using UnityEngine;

public class PlayerStateIdle : State<PlayerController>
{
    private Rigidbody2D rb2d;

    private float keyHorizontal;
    private bool keyJump = false;
    private bool keyShoot = false;

    public PlayerStateIdle(
        PlayerController controller, 
        FiniteStateMachine<PlayerController> fsm) : base(controller, fsm)
    {}

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
        if (keyHorizontal != 0f) mFsm.ChangeState(mController.runState);
        if (keyJump && mController.isGrounded) mFsm.ChangeState(mController.jumpState);
        if (keyShoot) mFsm.ChangeState(mController.shootState);

    }

    public override void OnStart()
    {
        base.OnStart();
        rb2d = mController.transform.GetComponent<Rigidbody2D>();

        rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
    }

    public override void OnStop()
    {
        base.OnStop();
    }
}
