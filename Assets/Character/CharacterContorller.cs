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
    public float sprintMultiplier = 1.5f;

    [Header("Smoothness")]
    public float acceleration = 10f;
    public float deceleration = 12f;

    private float currentSpeed = 0f;
    private Vector3 airVelocity;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float jumpDelay = 0.08f;
    public float jumpCooldown = 1f;
    private float lastJumpTime = -999f;

    [Header("Swim")]
    public bool Swim = false;

    public GameObject SwimFogFS;

    public float swimSpeed = 3f;
    public float swimUpDownSpeed = 3f;

    public float swimAcceleration = 2f;
    public float swimDeceleration = 3f;

    public float waterExitForce = 6f;

    [Header("Water Rules")]
    public float waterExitLockTime = 1f; // 🔥 время запрета выхода

    public float waterVerticalDamping = 4f;
    public float idleSinkDelay = 0.3f;
    public float idleSinkSpeed = 0.5f;

    private float waterEnterTime = -999f;
    private bool canExitWater = true;
    private float swimIdleTimer = 0f;

    private Vector3 swimVelocity;
    private bool wasSwimming = false;
    private bool applyWaterExitForce = false;

    [Header("Visual")]
    public float swimScaleY = 0.5f;

    private Vector3 originalScale;
    private float originalColliderHeight;
    private CapsuleCollider capsule;

    [Header("Mouse")]
    private float mouseSensitivity = 100f;
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
    public bool isSprinting;

    public Vector3 RevivePosition;



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

        jumpAction.performed += ctx => TryJump();

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
        RenderSettings.fog = true;

        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalScale = transform.localScale;
        if (capsule != null)
            originalColliderHeight = capsule.height;

        Cursor.lockState = CursorLockMode.Locked;


        // LOAD

        if (PlayerPrefs.HasKey("playerData" + PlayerPrefs.GetInt("WorldIndex", 0)))
        {
            string value = PlayerPrefs.GetString(
                "playerData" + PlayerPrefs.GetInt("WorldIndex", 0)
            );

            string[] split = value.Split('|');

            Swim = bool.Parse(split[17]);

            capsule.height = float.Parse(split[19]);

            transform.position = new Vector3(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2])
            );

            transform.eulerAngles = new Vector3(
                float.Parse(split[3]),
                float.Parse(split[4]),
                float.Parse(split[5])
            );

            rb.linearVelocity = new Vector3(
                float.Parse(split[6]),
                float.Parse(split[7]),
                float.Parse(split[8])
            );

            xRotation = float.Parse(split[9]);

            currentSpeed = float.Parse(split[10]);

            airVelocity = new Vector3(
                float.Parse(split[11]),
                float.Parse(split[12]),
                float.Parse(split[13])
            );

            swimVelocity = new Vector3(
                float.Parse(split[14]),
                float.Parse(split[15]),
                float.Parse(split[16])
            );

            Vector3 scale = transform.localScale;
            scale.y = float.Parse(split[18]);
            transform.localScale = scale;

            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }


        string value2 = PlayerPrefs.GetString("RevivePosition" + PlayerPrefs.GetInt("WorldIndex", 0), "0|0|0");
        string[] split2 = value2.Split('|');

        Vector3 pos2 = new Vector3(
            float.Parse(split2[0]),
            float.Parse(split2[1]),
            float.Parse(split2[2])
        );

        RevivePosition = pos2;
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        rb.useGravity = !Swim;

        Look();
        CheckGround();
        UpdateAnimation();
        HandleSwimState();
        HandleSwimVisual();

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);

        // 🔥 разрешаем выход из воды через 1 сек
        if (Swim && Time.time > waterEnterTime + waterExitLockTime)
            canExitWater = true;

        bool inWater = IsTouchingWater();

        if (inWater)
        {
            Swim = true;
            SwimFogFS.SetActive(true);
            RenderSettings.fogDensity = 0.115f;
            RenderSettings.fogColor = new Color(12f/255, 31f/255, 37f/255);
        }
        else if (!inWater)
        {
            Swim = false;
            SwimFogFS.SetActive(false);
            RenderSettings.fogDensity = 0.077f;
            RenderSettings.fogColor = new Color(0f/255, 0f/255, 0f/255);
        }
    }

    void FixedUpdate()
    {
        HandleWaterPhysics();

        if (applyWaterExitForce)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * waterExitForce, ForceMode.Impulse);
            applyWaterExitForce = false;
        }

        Move();
    }

    // ---------------- SWIM STATE ----------------

    void HandleSwimState()
    {
        if (Swim && !wasSwimming)
        {
            waterEnterTime = Time.time;
            canExitWater = false;

            swimVelocity = airVelocity;
            swimIdleTimer = 0f;
        }

        if (!Swim && wasSwimming)
        {
            // 🔥 запрет выхода если меньше 1 секунды
            if (!canExitWater)
            {
                Swim = true;
                return;
            }

            airVelocity = swimVelocity;
            currentSpeed = swimVelocity.magnitude;

            if (!isGrounded)
                applyWaterExitForce = true;
        }

        wasSwimming = Swim;
    }

    // ---------------- WATER PHYSICS ----------------

    void HandleWaterPhysics()
    {
        if (!Swim) return;

        float verticalInput = 0f;

        if (jumpAction.IsPressed() && canExitWater)
            verticalInput = 1f;
        else if (sprintAction.IsPressed())
            verticalInput = -1f;

        bool moving =
            moveInput.sqrMagnitude > 0.01f ||
            Mathf.Abs(verticalInput) > 0.01f;

        if (moving)
            swimIdleTimer = 0f;
        else
            swimIdleTimer += Time.fixedDeltaTime;

        Vector3 vel = rb.linearVelocity;

        float targetY = verticalInput * swimUpDownSpeed;

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            vel.y = Mathf.MoveTowards(vel.y, targetY, waterVerticalDamping * Time.fixedDeltaTime);
        }
        else
        {
            float sink = 0f;

            if (swimIdleTimer > idleSinkDelay)
                sink = -idleSinkSpeed;

            vel.y = Mathf.MoveTowards(vel.y, sink, waterVerticalDamping * Time.fixedDeltaTime);
        }

        rb.linearVelocity = vel;
    }

    // ---------------- MOVE ----------------

    void Move()
    {
        if (Swim)
        {
            SwimMove();
            return;
        }

        Vector3 dir = transform.right * moveInput.x + transform.forward * moveInput.y;
        dir = Vector3.ClampMagnitude(dir, 1f);

        if (isGrounded)
        {
            float targetSpeed = dir.magnitude > 0.1f
                ? (isSprinting ? moveSpeed * sprintMultiplier : moveSpeed)
                : 0f;

            float accel = targetSpeed > currentSpeed ? acceleration : deceleration;

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

            Vector3 move = dir * currentSpeed;
            airVelocity = move;

            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + airVelocity * Time.fixedDeltaTime);
        }
    }

    void SwimMove()
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        Vector3 dir = forward * moveInput.y + right * moveInput.x;

        float vertical = 0f;

        if (jumpAction.IsPressed() && canExitWater)
            vertical = 1f;
        else if (sprintAction.IsPressed())
            vertical = -1f;

        dir += Vector3.up * vertical;
        dir = Vector3.ClampMagnitude(dir, 1f);

        Vector3 target = dir * swimSpeed;

        float accel = target.magnitude < swimVelocity.magnitude
            ? swimDeceleration
            : swimAcceleration;

        swimVelocity = Vector3.MoveTowards(swimVelocity, target, accel * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + swimVelocity * Time.fixedDeltaTime);
    }

    // ---------------- OTHER ----------------

    void TryJump()
    {
        if (Swim) return;
        if (!isGrounded) return;
        if (Time.time < lastJumpTime + jumpCooldown) return;

        StartCoroutine(Jump());
        lastJumpTime = Time.time;
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(jumpDelay);

        if (isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Look()
    {
        float mx = lookInput.x * mouseSensitivity * Time.deltaTime;
        float my = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= my;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mx);
    }

    void CheckGround()
    {
        if (Swim) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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
            animator.SetInteger("Speed", 0);
        else if (isSprinting)
            animator.SetInteger("Speed", 2);
        else
            animator.SetInteger("Speed", 1);
    }

    void HandleSwimVisual()
    {
        float t = 4f;

        float targetY = Swim ? originalScale.y * swimScaleY : originalScale.y;

        Vector3 s = transform.localScale;
        s.y = Mathf.Lerp(s.y, targetY, Time.deltaTime * t);
        transform.localScale = s;

        if (capsule != null)
        {
            float h = Swim ? originalColliderHeight * swimScaleY : originalColliderHeight;
            capsule.height = Mathf.Lerp(capsule.height, h, Time.deltaTime * t);
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    bool IsTouchingWater()
    {
        Vector3 point1 = transform.position + Vector3.up * (capsule.height / 2f - capsule.radius);
        Vector3 point2 = transform.position - Vector3.up * (capsule.height / 2f - capsule.radius);

        return Physics.CheckCapsule(point1, point2, capsule.radius, LayerMask.GetMask("Water"));
    }

    public void Save()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        Vector3 vel = rb.linearVelocity;

        string value =
            pos.x + "|" + pos.y + "|" + pos.z + "|" +
            rot.x + "|" + rot.y + "|" + rot.z + "|" +
            vel.x + "|" + vel.y + "|" + vel.z + "|" +
            xRotation + "|" +
            currentSpeed + "|" +
            airVelocity.x + "|" + airVelocity.y + "|" + airVelocity.z + "|" +
            swimVelocity.x + "|" + swimVelocity.y + "|" + swimVelocity.z + "|" +
            Swim + "|" +
            transform.localScale.y + "|" +
            capsule.height;

        PlayerPrefs.SetString("playerData" + PlayerPrefs.GetInt("WorldIndex", 0), value);

        string value2 = RevivePosition.x + "|" + RevivePosition.y + "|" + RevivePosition.z;
        PlayerPrefs.SetString("RevivePosition" + PlayerPrefs.GetInt("WorldIndex", 0), value2);
    }

    public void Revive()
    {
        if(RevivePosition != new Vector3(0, 0, 0))
            transform.position = RevivePosition;
        else
            transform.position = new Vector3(143, 26.5f, 210);
    }
}