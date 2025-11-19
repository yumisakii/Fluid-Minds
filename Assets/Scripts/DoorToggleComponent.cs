using UnityEngine;

public class DoorToggleComponent : BaseToggleComponent
{
    [SerializeField] private Animator doorAnimator;
    private Material buttonMat;

    private void Start()
    {
        buttonMat = GetComponent<Renderer>().material;
        buttonMat.color = Color.red;

    }

    protected override void ActivateComponent()
    {
        buttonMat.color = Color.green;
        doorAnimator.SetBool("IsDoorOpen", true);
    }

    protected override void DeactivateComponent()
    {
        buttonMat.color = Color.red;
        doorAnimator.SetBool("IsDoorOpen", false);
    }
}
