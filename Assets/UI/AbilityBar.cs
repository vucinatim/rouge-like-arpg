using UnityEngine;

public class AbilityBar : MonoBehaviour
{
    [SerializeField] private AbilitySlot[] abilitySlots;

    public void AssignAbilitySlotKeys(KeyCode[] keys)
    {
        if (keys.Length != abilitySlots.Length)
        {
            Debug.LogError("Invalid key array length.");
            return;
        }

        for (int i = 0; i < keys.Length; i++)
        {
            abilitySlots[i].SetAssignedKey(keys[i]);
        }
    }

    public void AssignAbilityToSlot(Ability ability, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilitySlots.Length)
        {
            Debug.LogError("Invalid slot index.");
            return;
        }

        abilitySlots[slotIndex].AssignAbility(ability);
    }

    public void UpdateCooldownUI(int slotIndex, float cooldownRemaining, float totalCooldown)
    {
        if (slotIndex < 0 || slotIndex >= abilitySlots.Length)
        {
            Debug.LogError("Invalid slot index.");
            return;
        }

        abilitySlots[slotIndex].UpdateCooldownUI(cooldownRemaining, totalCooldown);
    }
}
