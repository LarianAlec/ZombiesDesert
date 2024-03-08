using UnityEngine;

[RequireComponent(typeof(PlayerMovementComponent))]
public class AnimationHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementComponent movementComponent;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        movementComponent = GetComponent<PlayerMovementComponent>();
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        float xVelocity = Vector3.Dot(movementComponent.GetMovementDirection().normalized, transform.right);
        float zVelocity = Vector3.Dot(movementComponent.GetMovementDirection().normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool isRunning = movementComponent.GetMovementDirection().magnitude > 0.15f && movementComponent.IsRunningKeyPressed();
        animator.SetBool("isRunning", isRunning);
    }
}
