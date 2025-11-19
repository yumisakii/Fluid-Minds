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
    
    private IHydroInteractable currentHitTarget;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction switchModeAction;

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
            HandleShooting();

        else if (context.canceled)
            HandleStopShooting();
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
