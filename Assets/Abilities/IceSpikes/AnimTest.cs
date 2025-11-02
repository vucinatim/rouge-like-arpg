using UnityEngine;
using DG.Tweening;

public class AnimTestWithSequence : MonoBehaviour
{
    private Vector3 startPosition;

    private void Start()
    {
        // Save the initial position of the cube
        startPosition = transform.position;

    }

    void Update()
    {
        // If "H" is pressed, animate the cube
        if (Input.GetKeyDown(KeyCode.H))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);

            AnimateTest();
            Debug.Log("H pressed. Animating cube.");
        }
    }

    public void AnimateTest()
    {
        float sizeMultiplier = 1f; // Simulate the size multiplier
        float riseDuration = 0.3f;
        float sinkDelay = 0.5f;
        float sinkDuration = 0.3f;

        Debug.Log($"Animating cube from {transform.position.y} to {startPosition.y}");

        // Create a DOTween sequence
        Sequence sequence = DOTween.Sequence();

        // Animate rising
        sequence.Append(transform.DOMoveY(startPosition.y + sizeMultiplier, riseDuration)
            .SetEase(Ease.OutBounce));

        // Wait and then sink
        sequence.AppendInterval(sinkDelay);
        sequence.Append(transform.DOMoveY(startPosition.y, sinkDuration)
            .SetEase(Ease.InBack));

        // Destroy or reset after sinking
        sequence.OnComplete(() =>
        {
            Debug.Log("Cube animation complete. Resetting cube.");
            transform.position = startPosition; // Reset the position for repeated testing
        });
    }
}
