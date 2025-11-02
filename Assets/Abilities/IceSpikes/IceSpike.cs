using UnityEngine;
using DG.Tweening;
using System;

public class IceSpike : MonoBehaviour
{
    private float sizeMultiplier;
    private Vector3 startPosition;

    public event Action<Collider> OnSpikeHit; // Event triggered on collision

    public void Initialize(float sizeMultiplier)
    {
        this.sizeMultiplier = sizeMultiplier;

        // Scale the spike
        transform.localScale = Vector3.one * sizeMultiplier;

        // Hide the spike below the ground and set startPosition after adjustment
        transform.position = new Vector3(transform.position.x, transform.position.y - sizeMultiplier, transform.position.z);
        startPosition = transform.position + new Vector3(0, sizeMultiplier * 0.9f, 0); // Correctly set start position
    }

    public void AnimateSpike()
    {
        float riseDuration = 0.2f; // Faster rise
        float sinkDelay = 0.2f;    // Short delay before sinking
        float sinkDuration = 0.2f; // Fast sink

        Debug.Log($"Animating spike from {transform.position.y} to {startPosition.y}");

        Sequence sequence = DOTween.Sequence();

        // Animate rising with a snappy ease
        sequence.Append(transform.DOMoveY(startPosition.y, riseDuration).SetEase(Ease.OutQuad));

        // Wait and then sink quickly
        sequence.AppendInterval(sinkDelay);
        sequence.Append(transform.DOMoveY(startPosition.y - sizeMultiplier, sinkDuration).SetEase(Ease.InQuad));

        // Destroy the spike after sinking
        sequence.OnComplete(() =>
        {
            Debug.Log("Spike animation complete. Destroying spike.");
            Destroy(gameObject);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        OnSpikeHit?.Invoke(other);
        Destroy(gameObject); // Destroy on collision
    }
}
