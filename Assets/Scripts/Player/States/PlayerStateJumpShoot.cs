using UnityEngine;

public class PlayerStateJumpShoot : State<PlayerController>
{
    private Animator animator;
    public PlayerStateJumpShoot(
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
        animator = mController.transform.GetComponent<Animator>();

        animator.Play("model-zx_jumpShoot");
    }

    public override void OnStop()
    {
        base.OnStop();
    }
}
