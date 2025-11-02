using UnityEngine;

[CreateAssetMenu(fileName = "New Fireball Ability", menuName = "Abilities/Fireball")]
public class FireballAbility : Ability
{
    public GameObject projectilePrefab; // The fireball prefab
    public float baseDamage = 10f;
    public float projectileSpeed = 15f;
    public float projectileLifetime = 2f;

    public override void TriggerAbility(AbilityHolder abilityHolder)
    {
        Debug.Log($"{abilityName} triggered!");

        // Get the player's position and rotation
        Transform projectileSpawnPoint = abilityHolder.GetProjectileSpawnPoint();

        // Fire the projectile
        GameObject fireball = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Projectile projectile = fireball.GetComponent<Projectile>();
        projectile.OnHit += OnFireballHit;
        projectile.Fire(abilityHolder.gameObject, projectileSpeed, projectileLifetime);
    }

    private void OnFireballHit(Collider collider)
    {
        EnemyHealth enemyHealth = collider.gameObject.GetComponent<EnemyHealth>();
        // Check if collider object Enemy Health script
        if (enemyHealth != null)
        {
            // Damage the enemy
            enemyHealth.ApplyDamage(baseDamage);
        }
    }
}
