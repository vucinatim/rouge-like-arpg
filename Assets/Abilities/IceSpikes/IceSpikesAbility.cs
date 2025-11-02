using UnityEngine;

[CreateAssetMenu(fileName = "New Ice Spikes Ability", menuName = "Abilities/Ice Spikes")]
public class IceSpikesAbility : Ability
{
    public int spikeCount = 10; // Number of spikes
    public float maxDistance = 9f; // Max distance for the cone
    public float spawnDelay = 0.05f; // Delay between each spike spawning
    public float coneAngle = 30f; // Angle of the cone
    public float spikeSize = 1f; // Size of the spikes
    public float baseDamage = 15f; // Damage per spike

    [Header("Spike Appearance")]
    public int spikeSides = 3; // Number of sides for each spike base
    public Material spikeMaterial; // Material for the spikes

    public override void TriggerAbility(AbilityHolder abilityHolder)
    {
        Debug.Log($"{abilityName} triggered!");

        Transform spawnPoint = abilityHolder.GetProjectileSpawnPoint();
        Vector3 origin = new Vector3(spawnPoint.position.x, 0, spawnPoint.position.z);
        Vector3 forward = spawnPoint.forward.normalized;

        abilityHolder.StartCoroutine(SpawnSpikes(origin, forward, abilityHolder));
    }

    private System.Collections.IEnumerator SpawnSpikes(Vector3 origin, Vector3 forward, AbilityHolder abilityHolder)
    {
        var randomSize = Random.Range(spikeSize, spikeSize + 0.2f);
        IceCrystalGenerator generator = new IceCrystalGenerator(spikeSides, spikeSize * 0.3f, randomSize, spikeMaterial);

        for (int i = 0; i < spikeCount; i++)
        {
            // Calculate the distance for the current spike
            float distance = (i / (float)spikeCount) * maxDistance;

            // Randomize within the cone angle
            float randomAngle = Random.Range(-coneAngle / 2, coneAngle / 2);

            // Calculate position and direction
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0) * Quaternion.LookRotation(forward);
            Vector3 spawnPosition = origin + rotation * Vector3.forward * distance;

            // Calculate tilt angle based on proximity to the player
            float tiltAngle = Mathf.Lerp(60f, 10f, i / (float)spikeCount);

            // Generate the spike object
            GameObject spikeInstance = generator.Generate(spawnPosition, rotation);
            spikeInstance.transform.Rotate(new Vector3(tiltAngle, 0, 0)); // Tilt the spike forward

            // Attach IceSpike logic
            BoxCollider boxCollider = spikeInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            IceSpike spikeScript = spikeInstance.AddComponent<IceSpike>();
            spikeScript.Initialize(spikeSize);
            spikeScript.AnimateSpike();

            // Subscribe to hit events
            spikeScript.OnSpikeHit += collider => HandleSpikeHit(collider, abilityHolder);

            // Wait before spawning the next spike
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void HandleSpikeHit(Collider collider, AbilityHolder abilityHolder)
    {
        EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.ApplyDamage(baseDamage);
            Debug.Log($"Ice Spike hit {collider.name}, dealing {baseDamage} damage.");
        }
    }
}
