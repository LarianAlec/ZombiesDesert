using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    private Player player;
    private PlayerMovementComponent movementComponent;

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
        movementComponent = player.GetMovementComponent();
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        float xVelocity = movementComponent.GetXVelocity();
        float zVelocity = movementComponent.GetZVelocity();

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool isRunning = movementComponent.GetMovementDirection().magnitude > 0.15f && movementComponent.IsRunningKeyPressed();
        animator.SetBool("isRunning", isRunning);
    }

    public void RunShootAnimation()
    {
        animator.SetTrigger("MakeShoot");
    }
}
