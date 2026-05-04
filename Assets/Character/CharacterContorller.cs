using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterContorller : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.8f;

    [Header("Smoothness")]
    public float acceleration = 10f;
    public float deceleration = 12f;

    private float currentSpeed = 0f;

    [Header("Air Movement")]
    public float airDrag = 1.5f;
    public float airBrakeMultiplier = 3f;

    private Vector3 airVelocity;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float jumpDelay = 0.08f;

    [Header("Swim")]
    public bool Swim = false;
    public float swimSpeed = 3f;
    public float swimUpDownSpeed = 3f;
    public float swimScaleY = 0.5f;

    // 🔥 НОВОЕ — плавность в воде
    public float swimAcceleration = 2f;
    public float swimDeceleration = 3f;

    private Vector3 swimVelocity;

    private Vector3 originalScale;
    private float originalColliderHeight;
    private CapsuleCollider capsule;

    [Header("Mouse")]
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform cameraPivot;
    public Animator animator;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation = 0f;
    private bool isGrounded;
    private bool isSprinting;

    void Awake()
    {
        var playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");
        sprintAction = playerMap.FindAction("Sprint");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();

        jumpAction.performed += ctx => StartCoroutine(JumpWithDelay());

        sprintAction.performed += ctx => isSprinting = true;
        sprintAction.canceled += ctx => isSprinting = false;
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalScale = transform.localScale;
        if (capsule != null)
            originalColliderHeight = capsule.height;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        rb.useGravity = !Swim;

        Look();
        CheckGround();
        UpdateAnimation();
        HandleSwimTransform();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (Swim)
        {
            SwimMove();
            return;
        }

        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        if (isGrounded)
        {
            float targetSpeed = moveDir.magnitude > 0.1f
                ? (isSprinting ? moveSpeed * sprintMultiplier : moveSpeed)
                : 0f;

            float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

            Vector3 move = moveDir * currentSpeed;
            airVelocity = move;

            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
        else
        {
            Vector3 inputDir = transform.right * moveInput.x + transform.forward * moveInput.y;
            inputDir = Vector3.ClampMagnitude(inputDir, 1f);

            if (inputDir.magnitude > 0.1f)
            {
                float dot = Vector3.Dot(airVelocity.normalized, inputDir.normalized);

                if (dot < 0f)
                {
                    airVelocity = Vector3.Lerp(airVelocity, Vector3.zero, airDrag * airBrakeMultiplier * Time.fixedDeltaTime);
                }
            }
            else
            {
                airVelocity = Vector3.Lerp(airVelocity, Vector3.zero, airDrag * Time.fixedDeltaTime);
            }

            rb.MovePosition(rb.position + airVelocity * Time.fixedDeltaTime);
        }
    }

    void SwimMove()
    {
        Transform cam = cameraPivot;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        Vector3 moveDir = forward * moveInput.y + right * moveInput.x;

        float vertical = 0f;

        if (jumpAction.IsPressed())
            vertical = 1f;
        else if (sprintAction.IsPressed())
            vertical = -1f;

        moveDir += Vector3.up * vertical;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        Vector3 targetVelocity = moveDir * swimSpeed;

        // 🔥 Плавное ускорение / торможение
        float accel = targetVelocity.magnitude > swimVelocity.magnitude ? swimAcceleration : swimDeceleration;

        swimVelocity = Vector3.MoveTowards(
            swimVelocity,
            targetVelocity,
            accel * Time.fixedDeltaTime
        );

        rb.MovePosition(rb.position + swimVelocity * Time.fixedDeltaTime);
    }

    void HandleSwimTransform()
    {
        float speed = 4f;

        float targetY = Swim ? originalScale.y * swimScaleY : originalScale.y;

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Lerp(scale.y, targetY, Time.deltaTime * speed);
        transform.localScale = scale;

        if (capsule != null)
        {
            float targetHeight = Swim ? originalColliderHeight * swimScaleY : originalColliderHeight;
            capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * speed);
        }
    }

    void UpdateAnimation()
    {
        if (Swim)
        {
            animator.SetInteger("Speed", swimVelocity.magnitude > 0.2f ? 1 : 0);
            return;
        }

        if (!isGrounded)
        {
            animator.SetInteger("Speed", 0);
            return;
        }

        if (currentSpeed < 0.1f)
        {
            animator.SetInteger("Speed", 0);
        }
        else if (isSprinting)
        {
            animator.SetInteger("Speed", 2);
        }
        else
        {
            animator.SetInteger("Speed", 1);
        }
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    IEnumerator JumpWithDelay()
    {
        if (!isGrounded || Swim) yield break;

        yield return new WaitForSeconds(jumpDelay);

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGround()
    {
        if (Swim) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}