using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float waterAmount = 0.5f; // How much water this bullet adds
    public float lifeTime = 5.0f;    // Destroy if it never hits anything

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Try to find the VoxelWater system in the scene
        // (You could also cache this if you have a singleton or reference manager)
        VoxelWater waterSystem = FindObjectOfType<VoxelWater>();

        if (waterSystem != null)
        {
            // 2. Calculate where we hit. 
            // We use the first contact point and move slightly explicitly *into* the hit
            // or just use the bullet's current position.
            Vector3 hitPosition = transform.position;

            // 3. Inject the water into the grid
            waterSystem.AddWater(hitPosition, waterAmount);
        }

        // 4. Destroy the bullet (splash effect!)
        Destroy(gameObject);
    }
}