using UnityEngine;

public class WaterToAbsorb : MonoBehaviour, IHydroInteractable
{
    [Header("Water Source Settings")]
    [SerializeField] private float waterAmount = 500f;
    [SerializeField] private float absorbRate = 20f;

    public void OnHydroHit(HydroCore playerCore, WaterState state, Vector3 hitPoint, Vector3 hitNormal)
    {
        // We only care if the gun is in the "Absorb" state
        if (state == WaterState.Absorb)
        {
            // Check if this source still has water
            if (waterAmount > 0)
            {
                // Calculate how much water to give this frame
                float waterToGive = absorbRate * Time.deltaTime;

                // Make sure we don't give more than we have
                if (waterToGive > waterAmount)
                {
                    waterToGive = waterAmount;
                }

                // Call the public "GainWater" method on the player's script
                playerCore.GainWater(waterToGive);

                // Remove the water from this source
                waterAmount -= waterToGive;
            }
        }
    }

    public void OnHydroHitStop(WaterState state)
    {

    }
}
