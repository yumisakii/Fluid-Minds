<<<<<<< HEAD
=======
using System.Collections;
>>>>>>> origin/levelIntro
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float speed = 5f;
<<<<<<< HEAD
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 20f;
=======
    [SerializeField] private float jumpForce = 50f;
    [SerializeField] private float rotationSpeed = 5f;
>>>>>>> origin/levelIntro
    [SerializeField] private Vector2 minMaxYaw = new(-90f, 90f);

    [Header("Player components")]
    [SerializeField] private Rigidbody rigidBody = null;
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private Transform root = null;
    [SerializeField] private Transform head = null;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.2f;

<<<<<<< HEAD
=======
    [Header("Interaction settings")]
    [SerializeField] private int rayDistance = 100;
    [SerializeField] private LayerMask interactionMask = default;

>>>>>>> origin/levelIntro
    private Vector3 moveInput = Vector3.zero;
    private Vector2 lookInput;
    private Vector2 currentRotation;
    private bool isGrounded;

    private void Reset()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Player_OnMove(CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Player_OnLook(CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Player_OnJump(CallbackContext context)
    {
        Vector3 rayStart = GetColliderBottom();
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundMask);

        if (context.performed && isGrounded)
        {
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, jumpForce, rigidBody.linearVelocity.z);
        }
    }

    public void Player_OnInteract(CallbackContext context)
    {
<<<<<<< HEAD
        // Player Interaction
=======
        if (!context.performed)
            return;
        Debug.Log("OUi");

        Ray ray = new Ray(head.position, head.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, interactionMask))
        {
            if (hit.collider.TryGetComponent(out InteractionToggleSetter interactionToggleSetter)) { 
                Debug.Log("Non");
                interactionToggleSetter.Interact();
            }
        }
>>>>>>> origin/levelIntro
    }

    private void LateUpdate()
    {
        currentRotation.x -= lookInput.y * rotationSpeed * Time.deltaTime;
        currentRotation.y += lookInput.x * rotationSpeed * Time.deltaTime;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minMaxYaw.x, minMaxYaw.y);

        root.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
        head.localRotation = Quaternion.Euler(currentRotation.x, 0, 0);
    }

    private void FixedUpdate()
    {
        Vector3 move = new(moveInput.x, 0, moveInput.y);
        Vector3 worldMove = root.rotation * move * speed;
        rigidBody.linearVelocity = new Vector3(worldMove.x, rigidBody.linearVelocity.y, worldMove.z);
    }

    private Vector3 GetColliderBottom()
    {
        Vector3 centerWorld = boxCollider.bounds.center;
        float bottomY = centerWorld.y - (boxCollider.size.y / 2f - 0.05f);
        return new Vector3(centerWorld.x, bottomY, centerWorld.z);
    }
}
