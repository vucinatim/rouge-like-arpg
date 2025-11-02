using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private bool isAttacking = false;

    [SerializeField] private AttackCollider attackCollider; // Reference to the AttackCollider script
    [SerializeField] private int attackDamage = 10; // Damage dealt to the player

    void Start()
    {
        // Initialize references
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Ensure the player GameObject has the 'Player' tag.");
        }

        // Subscribe to the attack collider event
        if (attackCollider != null)
        {
            attackCollider.OnTriggerHit += HandleTriggerHit;
            attackCollider.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("AttackCollider not assigned!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isAttacking)
        {
            // Stop movement while attacking
            agent.isStopped = true;
        }
        else if (distanceToPlayer <= agent.stoppingDistance && animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") == false)
        {
            StartCoroutine(AttackPlayer());
        }
        else
        {
            agent.SetDestination(player.position);

            // Play running animation if the zombie is moving
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);
        }

        // Sync the zombie's position with the NavMeshAgent
        SyncAgentPosition();
    }

    private IEnumerator AttackPlayer()
    {
        // Stop and prepare to attack
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        isAttacking = true;

        // Trigger the attack animation
        animator.SetTrigger("attack");

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(2f);

        // Resume chasing the player after attacking
        isAttacking = false;
        agent.isStopped = false;
    }

    private void EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.gameObject.SetActive(true);
        }
    }

    private void DisableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.gameObject.SetActive(false);
        }
    }

    private void HandleTriggerHit(Collider other)
    {
        Debug.Log("Zombie hit: " + other.name);
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void SyncAgentPosition()
    {
        // Match the character's position to the NavMeshAgent
        transform.position = agent.nextPosition;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (attackCollider != null)
        {
            attackCollider.OnTriggerHit -= HandleTriggerHit;
        }
    }
}
