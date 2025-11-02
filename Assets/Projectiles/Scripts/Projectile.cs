using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("General Settings")]
    public float speed = 15f;
    public float lifeTime = Mathf.Infinity;

    [Header("Hit Settings")]
    public float hitOffset = 0f;
    public GameObject hit;
    public GameObject[] Detached;

    [Header("Flash Settings")]
    public GameObject flash;

    [Header("Other")]
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = Vector3.zero;

    private Rigidbody rb;

    // Event triggered when the projectile hits something
    public event Action<Collider> OnHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop the projectile
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        // Trigger hit effects
        SpawnHitEffect(collision);

        // Detach any specified objects (e.g., trails)
        DetachObjects();

        // Trigger OnHit event
        OnHit?.Invoke(collision.collider);

        // Destroy the projectile
        Destroy(gameObject);
    }

    public void Fire(GameObject caster = null, float projectileSpeed = -1, float projectileLifeTime = Mathf.Infinity)
    {
        // Ignore collisions with the caster
        Collider casterCollider = caster?.GetComponent<Collider>();
        if (casterCollider != null)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), casterCollider);
        }
        // If no speed is provided, use the current speed
        this.speed = projectileSpeed > 0f ? projectileSpeed : this.speed;
        this.lifeTime = projectileLifeTime;

        SpawnFlashEffect();

        // Destroy the projectile after its lifetime
        if (lifeTime < Mathf.Infinity)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void SpawnFlashEffect()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = transform.forward;

            // Destroy flash effect after its duration
            DestroyAfterParticleDuration(flashInstance);
        }
    }

    private void SpawnHitEffect(Collision collision)
    {
        if (hit != null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point + contact.normal * hitOffset;

            var hitInstance = Instantiate(hit, position, rotation);

            // Adjust rotation if necessary
            if (UseFirePointRotation)
            {
                hitInstance.transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
            }
            else if (rotationOffset != Vector3.zero)
            {
                hitInstance.transform.rotation = Quaternion.Euler(rotationOffset);
            }
            else
            {
                hitInstance.transform.LookAt(contact.point + contact.normal);
            }

            // Destroy hit effect after its duration
            DestroyAfterParticleDuration(hitInstance);
        }
    }

    private void DetachObjects()
    {
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
    }

    private void DestroyAfterParticleDuration(GameObject particleObject)
    {
        var ps = particleObject.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(particleObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else if (particleObject.transform.childCount > 0)
        {
            var childPs = particleObject.transform.GetChild(0).GetComponent<ParticleSystem>();
            if (childPs != null)
            {
                Destroy(particleObject, childPs.main.duration + childPs.main.startLifetime.constantMax);
            }
        }
    }
}
