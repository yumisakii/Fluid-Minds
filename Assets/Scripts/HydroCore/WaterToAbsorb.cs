using UnityEngine;

public class WaterToAbsorb : MonoBehaviour, IHydroInteractable
{
    [Header("Water Source Settings")]
    [SerializeField] private float waterAmount = 500f;
    [SerializeField] private float absorbRate = 20f;

    public void OnHydroHit(HydroCore playerCore, WaterState state, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (state == WaterState.Absorb)
        {
            if (waterAmount > 0)
            {
                float waterToGive = absorbRate * Time.deltaTime;

                if (waterToGive > waterAmount)
                {
                    waterToGive = waterAmount;
                }

                playerCore.GainWater(waterToGive);
                waterAmount -= waterToGive;
            }
        }
    }

    public void OnHydroHitStop(WaterState state)
    {

    }
}
