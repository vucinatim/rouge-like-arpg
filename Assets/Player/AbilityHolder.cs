using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class AbilityHolder : NetworkBehaviour
{
    [Header("Equipped Abilities")]
    [SerializeField] private Ability[] equippedAbilities = new Ability[6];
    [SerializeField] private KeyCode[] abilityKeys = new KeyCode[6];

    [Header("Projectile Spawn Point")]
    [SerializeField] private GameObject projectileSpawnPoint;

    private AbilityBar abilityBar;
    // Cooldown tracking
    private Dictionary<int, float> abilityCooldowns = new Dictionary<int, float>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            abilityBar = HUD.Instance.abilityBar;
            abilityBar.AssignAbilitySlotKeys(abilityKeys);
            for (int i = 0; i < equippedAbilities.Length; i++)
            {
                EquipAbility(equippedAbilities[i], i);
            }
        }
    }

    private void Update()
    {
        // Update cooldowns
        UpdateCooldowns();

        // Handle ability input
        for (int i = 0; i < abilityKeys.Length; i++)
        {
            if (Input.GetKeyDown(abilityKeys[i]) && equippedAbilities[i] != null)
            {
                if (!IsAbilityOnCooldown(i))
                {
                    TriggerAbility(i);
                }
            }
        }
    }

    private void UpdateCooldowns()
    {
        for (var key = 0; key < equippedAbilities.Length; key++)
        {
            if (IsAbilityOnCooldown(key))
            {
                abilityCooldowns[key] -= Time.deltaTime;
                abilityBar?.UpdateCooldownUI(key, abilityCooldowns[key], equippedAbilities[key].baseCooldown);
            }
        }
    }

    private bool IsAbilityOnCooldown(int slotIndex)
    {
        return abilityCooldowns.ContainsKey(slotIndex) && abilityCooldowns[slotIndex] > 0;
    }

    private void TriggerAbility(int slotIndex)
    {
        Debug.Log($"Triggered ability: {equippedAbilities[slotIndex].abilityName}");

        // Trigger the ability's logic
        equippedAbilities[slotIndex].TriggerAbility(this);

        // Start the cooldown
        abilityCooldowns[slotIndex] = equippedAbilities[slotIndex].baseCooldown;
    }

    public void EquipAbility(Ability ability, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedAbilities.Length)
        {
            Debug.LogError("Invalid slot index.");
            return;
        }

        equippedAbilities[slotIndex] = ability;
        abilityBar?.AssignAbilityToSlot(ability, slotIndex);
    }

    public void UnequipAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedAbilities.Length)
        {
            Debug.LogError("Invalid slot index.");
            return;
        }

        equippedAbilities[slotIndex] = null;
        abilityCooldowns.Remove(slotIndex); // Remove cooldown for the slot
        abilityBar?.AssignAbilityToSlot(null, slotIndex);
    }

    public Transform GetProjectileSpawnPoint()
    {
        return projectileSpawnPoint.transform;
    }
}
