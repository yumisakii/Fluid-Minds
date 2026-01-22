using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject waterBulletPrefab; // Assign the prefab here
    public Transform firePoint;          // Where the bullet spawns (muzzle)
    public float shootForce = 15f;       // Velocity of the water
    public float fireRate = 0.1f;        // Time between shots

    private float nextFireTime = 0f;

    private void Update()
    {
        // Simple input check (Left Click)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (waterBulletPrefab == null || firePoint == null) return;

        // 1. Spawn the bullet
        GameObject bullet = Instantiate(waterBulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Add velocity
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Add force in the forward direction of the FirePoint
            rb.linearVelocity = firePoint.forward * shootForce;
        }
    }
}