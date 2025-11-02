using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public float baseCooldown;
    public AnimationClip animation;

    /// <summary>
    /// Trigger the ability logic. Called by PlayerAbilities.
    /// </summary>
    public abstract void TriggerAbility(AbilityHolder playerAbilities);
}
