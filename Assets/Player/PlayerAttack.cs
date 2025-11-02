using UnityEngine;
using System.Collections;
using FishNet.Object;

public class PlayerAttack : NetworkBehaviour
{
    private Animator characterAnimator;

    [Header("Attack Settings")]
    [SerializeField] private int slashDamage = 20; // Damage dealt by the slash
    [SerializeField] private float attackRange = 2.0f; // Maximum range of the attack
    [SerializeField] private float attackAngle = 45f; // Angle of the attack cone (in degrees)
    [SerializeField] private LayerMask enemyLayer; // Layer mask for detecting enemies
    [SerializeField] private float attackCooldown = 1.0f; // Time (in seconds) between attacks

    public float attackSpeed = 1f; // Attack animation speed multiplier

    private bool isAttackOnCooldown = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void Start()
    {
        characterAnimator = GetComponentInChildren<Animator>();
        characterAnimator.SetFloat("attackSpeed", 1f);
    }

    void Update()
    {
        // Update attack speed parameter in the Animator
        characterAnimator.SetFloat("attackSpeed", attackSpeed);

        // Skip attacking if rolling or currently attacking
        bool isRolling = characterAnimator.GetBool("roll");
        bool isAttacking = characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        if (isRolling || isAttacking || isAttackOnCooldown)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PerformSlash();
        }
    }

    private void PerformSlash()
    {
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("attack");
        }

        // Start the attack cooldown
        StartCoroutine(HandleAttackCooldown());

        // Start the coroutine to apply damage after a delay
        StartCoroutine(ApplySlashDamageAfterDelay(0.1f));
    }

    private IEnumerator HandleAttackCooldown()
    {
        isAttackOnCooldown = true;

        // Wait for the cooldown period, factoring in attack speed
        float cooldownTime = attackCooldown / attackSpeed;
        yield return new WaitForSeconds(cooldownTime);

        isAttackOnCooldown = false;
    }

    private IEnumerator ApplySlashDamageAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Detect enemies within range
        Collider[] potentialEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in potentialEnemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            // Check if the enemy is within the cone angle
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            if (angleToEnemy <= attackAngle / 2)
            {
                Debug.Log($"Hit: {enemy.name}");
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.ApplyDamage(slashDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack cone in the Scene view
        Gizmos.color = Color.red;

        Vector3 forward = transform.forward * attackRange;

        // Draw the two edges of the cone
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, attackAngle / 2, 0) * forward);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -attackAngle / 2, 0) * forward);

        // Draw the attack range sphere
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
