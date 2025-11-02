using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarForeground; // The health bar's foreground image

    private Coroutine healthBarAnimationCoroutine;

    /// <summary>
    /// Updates the health bar fill amount.
    /// </summary>
    /// <param name="currentHealth">The current health value.</param>
    /// <param name="maxHealth">The maximum health value.</param>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Smoothly animate the health bar UI
        if (healthBarAnimationCoroutine != null)
        {
            StopCoroutine(healthBarAnimationCoroutine);
        }
        healthBarAnimationCoroutine = StartCoroutine(AnimateHealthBar(currentHealth, maxHealth));
    }


    public void UpdateHealthBarInstantly(float currentHealth, float maxHealth)
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private IEnumerator AnimateHealthBar(float currentHealth, float maxHealth)
    {
        float targetValue = (float)currentHealth / maxHealth;
        float initialValue = healthBarForeground.fillAmount;
        float animationDuration = 0.1f; // Duration of the animation
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBarForeground.fillAmount = Mathf.Lerp(initialValue, targetValue, elapsedTime / animationDuration);
            yield return null;
        }

        // Ensure the final value is accurate
        healthBarForeground.fillAmount = targetValue;
    }

}
