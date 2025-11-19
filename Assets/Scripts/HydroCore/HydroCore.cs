using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HydroCore : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private float fireRange = 50f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float waterTank = 50;
    [SerializeField] private float maxWaterTank = 100;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tankText = null;

    [Header("Current State")]
    [SerializeField] private WaterState currentWaterState = WaterState.Jet; // Default state

    [Header("Water State Particle")]
    [SerializeField] private ParticleSystem jetParticle = null;
    [SerializeField] private ParticleSystem absorbParticle = null;

    private IHydroInteractable currentHitTarget;

    private bool isShootingJet = false;
    private bool isAbsorbing = false;


    private void Update()
    {
        if (isShootingJet)
        {
            currentWaterState = WaterState.Jet;
            HandleShooting(WaterState.Jet);
            UseWater(10 * Time.deltaTime);
        }
        else if (isAbsorbing)
        {
            currentWaterState = WaterState.Absorb;
            HandleShooting(WaterState.Absorb);
        }
        else
        {
            HandleStopShooting();
            if (jetParticle.isPlaying) jetParticle.Stop();
            if (absorbParticle.isPlaying) absorbParticle.Stop();
        }

        UpdateUI();
    }

    public void GainWater(float waterValue)
    {
        Debug.Log("Gained Water: " + waterValue);
        waterTank += waterValue;
        if (waterTank > maxWaterTank)
        {
            waterTank = maxWaterTank;
        }
    }

    private void UseWater(float waterValue)
    {
        waterTank -= waterValue;
        if (waterTank <= 0)
        {
            waterTank = 0;
            isShootingJet = false;
        }

    }

    private void UpdateUI()
    {
        tankText.text = Mathf.FloorToInt(waterTank).ToString();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && waterTank > 0)
        {
            isShootingJet = true;
            jetParticle.Play();
        }

        else if (context.canceled)
        {
            isShootingJet = false;
            jetParticle.Stop();
        }
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAbsorbing = true;
            absorbParticle.Play();
        }
        else if (context.canceled)
        {
            isAbsorbing = false;
            absorbParticle.Stop();
        }
    }

    private void HandleShooting(WaterState stateToFire)
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, fireRange, interactableLayers))
        {
            IHydroInteractable interactable = hit.collider.GetComponent<IHydroInteractable>();

            if (interactable != null)
            {
                interactable.OnHydroHit(this, currentWaterState, hit.point, hit.normal);

                currentHitTarget = interactable;
            }
            else
            {
                HandleStopShooting();
                currentHitTarget = null;
            }
        }
        else
        {
            HandleStopShooting();
            currentHitTarget = null;
        }
    }

    private void HandleStopShooting()
    {
        if (currentHitTarget != null)
        {
            currentHitTarget.OnHydroHitStop(currentWaterState);
            currentHitTarget = null;
        }
    }
}
