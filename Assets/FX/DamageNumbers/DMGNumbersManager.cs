using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DMGNumbersManager : MonoBehaviour
{
    [Header("Damage Number Prefab")]
    [SerializeField] private DamageNumber damageNumberPrefab; // Prefab for the damage number object

    [Header("Object Pool Settings")]
    [SerializeField] private int initialPoolSize = 20; // Starting size of the pool

    private Queue<DamageNumber> pool = new Queue<DamageNumber>();

    private void Start()
    {
        // Pre-populate the pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewDamageNumber();
        }
    }

    private DamageNumber CreateNewDamageNumber()
    {
        DamageNumber newDamageNumber = Instantiate(damageNumberPrefab, transform);
        newDamageNumber.Initialize(this);
        newDamageNumber.gameObject.SetActive(false);
        pool.Enqueue(newDamageNumber);
        return newDamageNumber;
    }

    public void SpawnDamageNumber(Vector3 position, float damage)
    {
        Debug.Log($"Spawning damage number at {position} with damage {damage}");
        DamageNumber damageNumber;
        int damageInt = Mathf.RoundToInt(damage);

        // Reuse from pool if available
        if (pool.Count > 0)
        {
            damageNumber = pool.Dequeue();
        }
        else
        {
            damageNumber = CreateNewDamageNumber();
        }

        // Activate and position the damage number
        damageNumber.gameObject.SetActive(true);
        damageNumber.transform.position = position;
        damageNumber.SetText(damageInt);

        // Play animation
        damageNumber.PlayAnimation();
    }

    public void ReturnToPool(DamageNumber damageNumber)
    {
        damageNumber.gameObject.SetActive(false);
        pool.Enqueue(damageNumber);
    }
}
