using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50; // Maximum health for the enemy
    private int currentHealth;

    [Header("Blood Effect Settings")]
    [SerializeField] private GameObject bloodDecalPrefab; // Blood effect prefab
    [SerializeField] private float bloodDecalDuration = 5f; // How long the blood decal lasts

    [Header("UI Settings")]
    [SerializeField] private GameObject healthBarPrefab; // Health bar prefab

    [SerializeField] private DMGNumbersManager damageNumbersManager; // Reference to the manager

    private HealthBar healthBar;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Get the Animator component
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Instantiate and initialize the health bar
        if (healthBarPrefab != null)
        {
            // Compute a new position for the health bar above the character
            Vector3 healthBarPosition = transform.position + Vector3.up * 2;
            GameObject healthBarObject = Instantiate(healthBarPrefab, healthBarPosition, Quaternion.identity, transform);
            healthBar = healthBarObject.GetComponent<HealthBar>();
            healthBar.UpdateHealthBarInstantly(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("Health bar prefab or position not assigned!");
        }
    }

    public void ApplyDamage(float damage)
    {
        // Reduce health and clamp to 0
        currentHealth -= Mathf.RoundToInt(damage);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        // Update health bar
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        // Spawn damage numbers
        if (damageNumbersManager != null)
        {
            damageNumbersManager.SpawnDamageNumber(transform.position + Vector3.up, damage);
        }

        // Play hit animation and disable the NavMeshAgent
        if (animator != null && navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            animator.SetTrigger("hit");

            // Start a coroutine to re-enable the NavMeshAgent after the animation
            StartCoroutine(ReenableNavMeshAgentAfterHit());
        }
        else
        {
            Debug.LogWarning("Animator or NavMeshAgent not assigned!");
        }

        // Spawn blood splatter
        SpawnBloodDecal();

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ReenableNavMeshAgentAfterHit()
    {
        // Get the duration of the hit animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float hitAnimationDuration = stateInfo.length;

        // Wait for the animation to finish
        yield return new WaitForSeconds(hitAnimationDuration);

        // Re-enable the NavMeshAgent
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
        }
    }

    private void SpawnBloodDecal()
    {
        if (bloodDecalPrefab != null)
        {
            // Generate random rotation and scale
            float randomYRotation = Random.Range(0f, 360f);
            float randomScale = Random.Range(0.5f, 1.0f);

            // Instantiate the decal prefab at the enemy's position
            GameObject bloodDecal = Instantiate(
                bloodDecalPrefab,
                transform.position + new Vector3(0, 0.5f, 0),
                Quaternion.Euler(90f, randomYRotation, 0f)
            );

            // Scale the decal randomly
            bloodDecal.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // Start fade-out Coroutine
            StartCoroutine(FadeOutAndDestroy(bloodDecal));
        }
        else
        {
            Debug.LogWarning("Blood decal prefab not assigned!");
        }
    }

    private IEnumerator FadeOutAndDestroy(GameObject bloodDecal)
    {
        float fadeDuration = bloodDecalDuration; // Duration over which the decal fades
        DecalProjector projector = bloodDecal.GetComponent<DecalProjector>();

        if (projector != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;

                // Gradually reduce the fade factor
                projector.fadeFactor = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

                yield return null;
            }
        }

        // Destroy the decal after fading out
        Destroy(bloodDecal);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // Play death animation, if any
        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        // Destroy the health bar
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        // Destroy the enemy after a short delay to allow death animation to play
        Destroy(gameObject, 2f);
    }
}
