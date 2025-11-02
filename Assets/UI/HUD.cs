using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; } // Singleton instance

    public AbilityBar abilityBar;
    public HealthBar healthBar;

    private void Awake()
    {
        // Ensure there is only one instance of HUD
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate HUD instances
            return;
        }

        Instance = this; // Assign this instance as the singleton instance
        DontDestroyOnLoad(gameObject); // Keep this object across scenes if necessary
    }

    private void OnDestroy()
    {
        // Clear the instance when the object is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
