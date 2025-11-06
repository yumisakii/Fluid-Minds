using UnityEngine;
using UnityEngine.InputSystem;

public class HydroCore : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private float fireRange = 50f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Transform raycastOrigin; // the camera

    [Header("Current State")]
    [SerializeField] private WaterState currentWaterState = WaterState.Jet; // Default state

    [Header("Water State Particle")]
    [SerializeField] private ParticleSystem jetParticle = null;

    private IHydroInteractable currentHitTarget;

    private bool isShootingJet = false;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction switchModeAction;


    private void Update()
    {
        if (isShootingJet)
            HandleShooting();
        else
            HandleStopShooting();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    private void HandleShooting()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, fireRange, interactableLayers))
        {
            IHydroInteractable interactable = hit.collider.GetComponent<IHydroInteractable>();

            if (interactable != null)
            {
                interactable.OnHydroHit(currentWaterState, hit.point, hit.normal);

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
