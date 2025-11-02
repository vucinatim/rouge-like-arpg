using UnityEngine;
using UnityEngine.AI;
using Animancer;
using System.Collections;

public class PlayerMovementAnimancer : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 6f; // Base movement speed
    public float rollSpeed = 15f; // Base roll speed

    [Header("Animations")]
    [SerializeField] private AnimationClip idleAnimation; // Idle animation
    [SerializeField] private AnimationClip runAnimation; // Running animation
    [SerializeField] private AnimationClip rollAnimation; // Rolling animation
    [SerializeField] private MixerTransition2D movementMixer; // Animancer transition for movement
    [SerializeReference]
    private ITransition _Animation;

    private NavMeshAgent agent;
    private AnimancerComponent animancer; // Animancer component
    private bool isRolling = false;

    private Vector3 currentMovementDirection = Vector3.zero; // Smoothed movement direction
    private float smoothTime = 0.05f; // Time for smoothing movement direction
    private Vector3 movementVelocity = Vector3.zero; // Velocity for SmoothDamp

    private AnimancerState currentAnimationState; // Tracks the current animation state

    void Start()
    {
        // Get the Animancer component
        animancer = GetComponentInChildren<AnimancerComponent>();

        // Get the NavMeshAgent from this GameObject
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation handling by the NavMeshAgent
        agent.speed = 0; // We control movement speed manually

        PlayAnimation(idleAnimation); // Start with the idle animation
    }

    void Update()
    {
        if (!isRolling)
        {
            HandleMovement();
            HandleRotation();
            HandleAnimations();
        }

        // Trigger roll animation
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling)
        {
            SnapToMousePosition(); // Snap rotation to mouse position
            StartCoroutine(HandleRoll());
        }
    }

    private void HandleMovement()
    {
        // Get WASD input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Target movement direction based on input
        Vector3 targetDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Smoothly adjust the current movement direction
        currentMovementDirection = Vector3.SmoothDamp(currentMovementDirection, targetDirection, ref movementVelocity, smoothTime);

        if (currentMovementDirection.magnitude > 0.01f)
        {
            // Move the character manually
            transform.Translate(currentMovementDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void HandleRotation()
    {
        // Smoothly rotate the player to face the mouse cursor
        RotateToMouse(Time.deltaTime * 10f);
    }

    private void SnapToMousePosition()
    {
        // Snap rotation to the mouse cursor instantly
        RotateToMouse(1f); // Pass 1 for instant rotation (ignores smoothing)
    }

    private void RotateToMouse(float rotationSpeed)
    {
        // Cast a ray from the mouse cursor to the world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = hit.point - transform.position;
            targetDirection.y = 0; // Ignore the y-axis

            if (targetDirection.magnitude > 0.1f) // Prevent jitter when mouse is very close
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
            }
        }
    }

    private void HandleAnimations()
    {
        // Calculate local movement direction relative to the player's facing direction
        bool isMoving = currentMovementDirection.magnitude > 0.1f;

        // Play appropriate animations based on movement state
        if (isMoving)
        {
            PlayAnimation(runAnimation); // Running animation
        }
        else
        {
            PlayAnimation(idleAnimation); // Idle animation
        }
    }

    private IEnumerator HandleRoll()
    {
        isRolling = true;

        // Play the roll animation
        PlayAnimation(rollAnimation);

        // Disable NavMeshAgent during the roll
        agent.enabled = false;

        // Calculate roll direction
        Vector3 rollDirection = transform.forward; // Roll forward based on current facing direction
        float rollDuration = rollAnimation.length; // Use animation length for roll duration

        // Move the player manually during the roll
        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            transform.Translate(rollDirection * rollSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Re-enable NavMeshAgent after the roll
        agent.enabled = true;

        isRolling = false;
    }

    private void PlayAnimation(AnimationClip clip, float fadeDuration = 0.1f)
    {
        if (clip == null)
        {
            Debug.LogError("[PlayerMovement]: Animation clip is null!");
            return;
        }

        // Play the animation if it's not already playing
        if (currentAnimationState == null || currentAnimationState.Clip != clip)
        {
            currentAnimationState = animancer.Play(clip, fadeDuration);
        }
    }
}
