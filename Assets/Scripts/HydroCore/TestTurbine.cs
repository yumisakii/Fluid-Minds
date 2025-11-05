using UnityEngine;

public class TestTurbine : MonoBehaviour, IHydroInteractable
{
    [Header("Turbine Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    private bool isBeingHitByJet = false;

    private void Update()
    {
        if (isBeingHitByJet)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnHydroHit(WaterState state, Vector3 hitPoint, Vector3 hitNormal)
    {        
        if (state == WaterState.Jet)
        {
            isBeingHitByJet = true;

            // add force to the hit point for more realisme (hiting the side makes it spin faster or somth)
            // GetComponent<Rigidbody>().AddForceAtPosition(hitNormal * -1f * forceAmount, hitPoint);
        }
    }
    public void OnHydroHitStop(WaterState state)
    {
        if (state == WaterState.Jet)
        {
            isBeingHitByJet = false;
        }
    }
}
