using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlot : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI keyButtonText;

    private KeyCode triggerKey;
    private Ability assignedAbility;

    public void SetAssignedKey(KeyCode key)
    {
        triggerKey = key;
        AssignKeyCode(key);
    }

    public KeyCode GetAssignedKey()
    {
        return triggerKey;
    }

    public void AssignAbility(Ability ability)
    {
        assignedAbility = ability;

        abilityIcon.sprite = ability != null ? ability.abilityIcon : null;
        abilityIcon.enabled = ability != null;

        ResetCooldownUI();
    }

    public void UpdateCooldownUI(float cooldownRemaining, float totalCooldown)
    {
        if (cooldownRemaining <= 0)
        {
            ResetCooldownUI();
        }
        else
        {
            float fillAmount = cooldownRemaining / totalCooldown;
            cooldownOverlay.fillAmount = fillAmount;
            cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
            cooldownOverlay.enabled = true;
            cooldownText.enabled = true;
        }
    }

    private void ResetCooldownUI()
    {
        cooldownOverlay.fillAmount = 0f;
        cooldownOverlay.enabled = false;
        cooldownText.text = "";
        cooldownText.enabled = false;
    }

    private void AssignKeyCode(KeyCode key)
    {
        keyButtonText.text = key == KeyCode.Mouse0 ? "<sprite=0>" :
                             key == KeyCode.Mouse1 ? "<sprite=1>" :
                             key.ToString();
    }
}
