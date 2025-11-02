using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro textMesh; // Reference to TextMeshPro component

    [Header("Animation Settings")]
    [SerializeField] private float floatUpDistance = 1f; // How far it floats up
    [SerializeField] private float fadeDuration = 1f; // How long it takes to fade out
    [SerializeField] private AnimationCurve floatCurve; // Optional for custom movement curve

    [Header("Color Settings")]
    [SerializeField] private Gradient damageGradient; // Gradient for damage color
    [SerializeField] private int minDamage = 0; // Minimum damage value for gradient
    [SerializeField] private int maxDamage = 100; // Maximum damage value for gradient

    private DMGNumbersManager manager;
    private Vector3 initialPosition;

    public void Initialize(DMGNumbersManager manager)
    {
        if (textMesh == null)
        {
            Debug.LogError("Damage numbers TextMeshPro component not assigned!");
        }
        this.manager = manager;
    }

    public void SetText(int damage)
    {
        textMesh.text = damage.ToString();
        textMesh.color = CalculateColorFromDamage(damage); // Set color based on damage
    }

    private Color CalculateColorFromDamage(int damage)
    {
        // Clamp damage to the range
        float normalizedDamage = Mathf.InverseLerp(minDamage, maxDamage, damage);
        return damageGradient.Evaluate(normalizedDamage);
    }

    public void PlayAnimation()
    {
        initialPosition = transform.position;
        StartCoroutine(AnimateAndReturn());
    }

    private IEnumerator AnimateAndReturn()
    {
        float elapsed = 0f;
        Color startColor = textMesh.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Float up
            float floatProgress = elapsed / fadeDuration;
            transform.position = initialPosition + Vector3.up * floatCurve.Evaluate(floatProgress) * floatUpDistance;

            // Fade out
            float alpha = Mathf.Lerp(1f, 0f, floatProgress);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Return to pool after animation ends
        manager.ReturnToPool(this);
    }
}
