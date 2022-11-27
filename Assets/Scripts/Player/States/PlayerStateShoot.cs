
using UnityEngine;

public class PlayerStateShoot : State<PlayerController>
{
    Animator animator;
    AudioSource audio_S;

    float shootTime;
    public PlayerStateShoot(
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
        
        mController.isShooting = true;
        mController.keyShootRelease = false;
        shootTime = Time.time;
        // Shoot Bullet
        mController.Invoke("ShootBullet", 0.1f);
        audio_S.clip = mController.sound[3];
        audio_S.Play();
    }

    public override void OnStart()
    {
        base.OnStart();
        animator = mController.GetComponent<Animator>();
        audio_S = mController.GetComponent<AudioSource>();

        animator.Play("model-zx_shoot");
    }

    public override void OnStop()
    {
        base.OnStop();
    }

    
}

