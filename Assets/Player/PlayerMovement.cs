using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 6f; // Base movement speed
    public float rollSpeed = 15f; // Base roll speed

    private NavMeshAgent agent;
    private Animator characterAnimator; // Animator on the child object
    private bool isRolling = false;

    private Vector3 currentMovementDirection = Vector3.zero; // Smoothed movement direction
    private float smoothTime = 0.05f; // Time for smoothing movement direction
    private Vector3 movementVelocity = Vector3.zero; // Velocity for SmoothDamp

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Disable player movement for all clients except the local player
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            return;
        }
    }

    void Start()
    {
        // Get the Animator from the child object
        characterAnimator = GetComponentInChildren<Animator>();

        // Get the NavMeshAgent from this GameObject
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation handling by the NavMeshAgent
        agent.speed = 0; // We control movement speed manually

        // Set default Animator parameters
        characterAnimator.SetFloat("moveSpeed", 1f);
        characterAnimator.SetFloat("rollSpeed", 1f);
    }

    void Update()
    {
        characterAnimator.SetFloat("moveSpeed", moveSpeed / 5f);
        characterAnimator.SetFloat("rollSpeed", rollSpeed / 15f);
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
        Vector3 localDirection = transform.InverseTransformDirection(currentMovementDirection);

        // Set movement-related animation parameters
        if (characterAnimator != null)
        {
            bool isMoving = currentMovementDirection.magnitude > 0.1f;
            characterAnimator.SetBool("isRunning", isMoving);

            // Pass local direction parameters to Animator
            characterAnimator.SetFloat("moveX", localDirection.x);
            characterAnimator.SetFloat("moveZ", localDirection.z);
        }
    }

    private IEnumerator HandleRoll()
    {
        isRolling = true;

        // Trigger the roll animation
        characterAnimator.SetTrigger("roll");

        // Disable NavMeshAgent during the roll
        agent.enabled = false;

        // Calculate roll direction
        Vector3 rollDirection = transform.forward; // Roll forward based on current facing direction
        float rollDuration = 0.5f / (rollSpeed / 15f); // Adjust roll duration to match rollSpeed

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
}
