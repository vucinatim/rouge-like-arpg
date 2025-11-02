using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using FishNet.Object;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    // Blood decal prefab to spawn when the player takes damage
    [SerializeField] private GameObject bloodDecalPrefab;

    // Duration of the blood decal before it disappears
    [SerializeField] private float bloodDecalDuration = 5f;

    private HealthBar healthBar;
    private Animator characterAnimator; // Animator on the child object

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            healthBar = HUD.Instance.healthBar;
            healthBar.UpdateHealthBarInstantly(currentHealth, maxHealth);
        }

    }

    void Start()
    {
        // Get the Animator from the child object
        characterAnimator = GetComponentInChildren<Animator>();

        // Initialize health
        currentHealth = maxHealth;
    }

    // Check key "T" to test the TakeDamage method
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce health and clamp it to 0
        currentHealth -= damage;

        characterAnimator.SetTrigger("hit");

        healthBar?.UpdateHealthBar(currentHealth, maxHealth);

        // Spawn blood splatter
        SpawnBloodDecal();

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void SpawnBloodDecal()
    {
        if (bloodDecalPrefab != null)
        {
            Debug.Log("Spawning blood decal!");

            // Generate random Y rotation and scale
            float randomYRotation = Random.Range(0f, 360f);
            float randomScale = Random.Range(0.5f, 1.0f); // Minimum 0.5f, maximum 1.0f

            // Instantiate the decal prefab at the player's position
            GameObject bloodDecal = Instantiate(
                bloodDecalPrefab,
                transform.position + new Vector3(0, 0.5f, 0),
                Quaternion.Euler(90f, randomYRotation, 0f)
            );

            // Apply random scale
            bloodDecal.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // Start fade-out Coroutine
            StartCoroutine(FadeOutAndDestroy(bloodDecal));
        }
        else
        {
            Debug.LogWarning("[PlayerHealth]: Blood decal prefab not assigned!");
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
        Debug.Log("Player is dead!");
        // Handle player death (e.g., show game over screen, respawn, etc.)
    }
}
