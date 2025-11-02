using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public System.Action<Collider> OnTriggerHit; // Event to notify when a collision happens

    private void OnTriggerEnter(Collider other)
    {
        // Notify any listener (e.g., parent enemy script) about the collision
        OnTriggerHit?.Invoke(other);
    }
}
