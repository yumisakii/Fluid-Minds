using UnityEngine;

public class TestTurbine : MonoBehaviour, IHydroInteractable
{
    [Header("Turbine Settings")]
    [SerializeField] private float rotationSpeed = 50f;

    [SerializeField] private Animator doorAnimator;

    private bool isBeingHitByJet = false;

    private float rotationValue = 0f;

    private void Update()
    {
        if (isBeingHitByJet)
        {
            transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
            rotationValue += rotationSpeed * Time.deltaTime;
        }

        if (rotationValue >= 360f)
        {
            Debug.Log("Turbine completed a full rotation!");
            doorAnimator.SetBool("IsDoorOpen", true);
            rotationValue = 0f;
        }
    }

    public void OnHydroHit(HydroCore hydroCore, WaterState state, Vector3 hitPoint, Vector3 hitNormal)
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
