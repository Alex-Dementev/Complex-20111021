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

    [Header("Jump")]
    public float jumpForce = 5f;
    public float jumpDelay = 0.08f;
    public float jumpCooldown = 1f;
    private float lastJumpTime = -999f;

    [Header("Swim")]
    public bool Swim = false;
    public GameObject SwimFogFS;
    public float swimSpeed = 4f;        
    public float swimUpDownSpeed = 3f;
    public float swimAcceleration = 2f;
    public float swimDeceleration = 1.5f; 
    public float waterExitForce = 6f;

    [Header("Water Rules")]
    public float waterExitLockTime = 1f; 
    public float waterVerticalDamping = 4f;
    public float idleSinkDelay = 0.3f;
    public float idleSinkSpeed = 0.5f;

    private float waterEnterTime = -999f;
    private bool canExitWater = true;
    private float swimIdleTimer = 0f;
    private bool wasSwimming = false;
    private bool applyWaterExitForce = false;

    private float waterExitCooldownTimer = 2f;

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
        sprintAction = playerMap.FindAction("Shift");
    }

    void OnEnable()
    {
        moveAction.Enable(); lookAction.Enable(); jumpAction.Enable(); sprintAction.Enable();
        jumpAction.performed += ctx => TryJump();
        sprintAction.performed += ctx => isSprinting = true;
        sprintAction.canceled += ctx => isSprinting = false;
    }

    void OnDisable()
    {
        moveAction.Disable(); lookAction.Disable(); jumpAction.Disable(); sprintAction.Disable();
    }

    void Start()
    {
        RenderSettings.fog = true;
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalScale = transform.localScale;
        if (capsule != null) originalColliderHeight = capsule.height;

        Cursor.lockState = CursorLockMode.Locked;

        if (PlayerPrefs.HasKey("playerData" + PlayerPrefs.GetInt("WorldIndex", 0)))
        {
            string value = PlayerPrefs.GetString("playerData" + PlayerPrefs.GetInt("WorldIndex", 0));
            string[] split = value.Split('|');

            Swim = bool.Parse(split[17]);
            capsule.height = float.Parse(split[19]);
            transform.position = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            transform.eulerAngles = new Vector3(float.Parse(split[3]), float.Parse(split[4]), float.Parse(split[5]));
            rb.linearVelocity = new Vector3(float.Parse(split[6]), float.Parse(split[7]), float.Parse(split[8]));
            xRotation = float.Parse(split[9]);
            currentSpeed = float.Parse(split[10]);
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        string value2 = PlayerPrefs.GetString("RevivePosition" + PlayerPrefs.GetInt("WorldIndex", 0), "0|0|0");
        string[] split2 = value2.Split('|');
        RevivePosition = new Vector3(float.Parse(split2[0]), float.Parse(split2[1]), float.Parse(split2[2]));
    }

    private void OnCollisionEnter(Collision collision)
    {
        float verticalSpeed = Mathf.Abs(collision.relativeVelocity.y);
        
        Vector3 horizontalVector = new Vector3(collision.relativeVelocity.x, 0, collision.relativeVelocity.z);
        float horizontalSpeed = horizontalVector.magnitude;

        int totalDamage = 0;

        if(verticalSpeed > 8f)
        {
            float extraVertical = verticalSpeed - 8f;
            totalDamage += Mathf.RoundToInt(extraVertical * 3f); 

            if(verticalSpeed > 13)
            {
                extraVertical = verticalSpeed - 5f;
                totalDamage += Mathf.RoundToInt(extraVertical * 5f); 
                Debug.Log("Высокое падение " + verticalSpeed);
            }
            else
                Debug.Log("Падение " + verticalSpeed);
        }

        if(horizontalSpeed > 9f)
        {
            float extraHorizontal = horizontalSpeed - 9f;
            totalDamage += Mathf.RoundToInt(extraHorizontal * 4f);
            Debug.Log("Влетел в объект на большой скорости");
        }

        if(totalDamage > 0)
            SystemsController.Heals = SystemsController.Heals - totalDamage;
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

        if (Swim && Time.time > waterEnterTime + waterExitLockTime)
            canExitWater = true;

        bool inWater = IsTouchingWater();

        if (inWater)
        {
            Swim = true;
            SwimFogFS.SetActive(true);
            RenderSettings.fogDensity = 0.085f;
            RenderSettings.fogColor = new Color(12f/255, 31f/255, 37f/255);

            if (waterExitCooldownTimer > 0f)
                waterExitCooldownTimer -= Time.deltaTime;
        }
        else
        {
            Swim = false;
            SwimFogFS.SetActive(false);
            RenderSettings.fogDensity = 0.060f;
            RenderSettings.fogColor = new Color(0f/255, 0f/255, 0f/255);

            waterExitCooldownTimer = 2f;
        }
    }

    void FixedUpdate()
    {
        if (applyWaterExitForce)
        {
            // Направление вылета вперед берется от поворота камеры
            Vector3 forwardDirection = new Vector3(cameraPivot.forward.x, 0f, cameraPivot.forward.z).normalized;
            rb.AddForce(Vector3.up * waterExitForce + forwardDirection * (swimSpeed * 1.2f), ForceMode.Impulse);
            applyWaterExitForce = false;
        }

        ProcessMovement();
    }

    void HandleSwimState()
    {
        if (Swim && !wasSwimming)
        {
            waterEnterTime = Time.time;
            canExitWater = false;
            swimIdleTimer = 0f;
        }

        if (!Swim && wasSwimming)
        {
            if (!canExitWater)
            {
                Swim = true;
                return;
            }

            if (!isGrounded)
                applyWaterExitForce = true;
        }

        wasSwimming = Swim;
    }

    // ---------------- ЕДИНАЯ СИСТЕМА ДВИЖЕНИЯ ----------------
    void ProcessMovement()
    {
        Vector3 currentVelocity = rb.linearVelocity;

        if (Swim)
        {
            // --- ЛОГИКА В ВОДЕ ---
            Vector3 forward = cameraPivot.forward;
            Vector3 right = cameraPivot.right;
            Vector3 targetDir = forward * moveInput.y + right * moveInput.x;

            float verticalInput = 0f;
            if (jumpAction.IsPressed() && canExitWater && waterExitCooldownTimer <= 0f)
                verticalInput = 1f;
            else if (sprintAction.IsPressed())
                verticalInput = -1f;

            targetDir += Vector3.up * verticalInput;
            targetDir = Vector3.ClampMagnitude(targetDir, 1f);

            float currentSwimMaxSpeed = isSprinting ? swimSpeed * sprintMultiplier : swimSpeed;
            Vector3 targetSwimVelocity = targetDir * currentSwimMaxSpeed;

            bool isMovingInput = moveInput.sqrMagnitude > 0.01f || Mathf.Abs(verticalInput) > 0.01f;
            if (isMovingInput) swimIdleTimer = 0f;
            else swimIdleTimer += Time.fixedDeltaTime;

            if (!isMovingInput && swimIdleTimer > idleSinkDelay)
            {
                targetSwimVelocity.y = -idleSinkSpeed;
            }
            float currentHorizontalMagnitude = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
            float targetHorizontalMagnitude = new Vector3(targetSwimVelocity.x, 0, targetSwimVelocity.z).magnitude;
            
            float xzAccel = targetHorizontalMagnitude < currentHorizontalMagnitude ? swimDeceleration : swimAcceleration;
            float yAccel = isMovingInput ? swimAcceleration : waterVerticalDamping;

            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetSwimVelocity.x, xzAccel * Time.fixedDeltaTime);
            currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, targetSwimVelocity.z, xzAccel * Time.fixedDeltaTime);
            currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetSwimVelocity.y, yAccel * Time.fixedDeltaTime);

            rb.linearVelocity = currentVelocity;
        }
        else
        {
            // --- ЛОГИКА НА СУШЕ / В ВОЗДУХЕ ---
            Vector3 dir = transform.right * moveInput.x + transform.forward * moveInput.y;
            dir = Vector3.ClampMagnitude(dir, 1f);

            if (isGrounded)
            {
                float targetSpeed = dir.magnitude > 0.1f ? (isSprinting ? moveSpeed * sprintMultiplier : moveSpeed) : 0f;
                float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

                Vector3 targetMove = dir * currentSpeed;
                rb.linearVelocity = new Vector3(targetMove.x, currentVelocity.y, targetMove.z);
            }
            else
            {
                // 🔥 РЕАЛИСТИЧНОЕ ЗАТУХАНИЕ СКОРОСТИ В ПОЛЕТЕ (Сопротивление воздуха)
                // Каждую секунду полета горизонтальная скорость плавно уменьшается на определенный процент.
                // Мягкое демпфирование (примерно деление на 1.2 к концу стандартной дуги прыжка):
                float airResistance = 0.5f; // Сила сопротивления воздуха. Подкрути, если гасится слишком быстро/медленно
                
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0f, airResistance * Time.fixedDeltaTime * Mathf.Abs(currentVelocity.x));
                currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, 0f, airResistance * Time.fixedDeltaTime * Mathf.Abs(currentVelocity.z));

                // Небольшой стрейф (управление) в воздухе, который накладывается поверх затухания, но не сбрасывает инерцию
                if (dir.magnitude > 0.1f)
                {
                    float airControlHorizontalSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
                    currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, dir.x * airControlHorizontalSpeed, acceleration * 0.2f * Time.fixedDeltaTime);
                    currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, dir.z * airControlHorizontalSpeed, acceleration * 0.2f * Time.fixedDeltaTime);
                }
                
                rb.linearVelocity = currentVelocity;
            }
        }
    }

    // ---------------- ОСТАЛЬНЫЕ МЕТОДЫ ----------------
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
        if (Swim) { isGrounded = false; return; }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void UpdateAnimation()
    {
        if (Swim) { animator.SetInteger("Speed", rb.linearVelocity.magnitude > 0.2f ? 1 : 0); return; }
        if (!isGrounded) { animator.SetInteger("Speed", 0); return; }
        if (currentSpeed < 0.1f) animator.SetInteger("Speed", 0);
        else if (isSprinting) animator.SetInteger("Speed", 2);
        else animator.SetInteger("Speed", 1);
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
        Vector3 pos = transform.position; Vector3 rot = transform.eulerAngles; Vector3 vel = rb.linearVelocity;
        
        string value = pos.x + "|" + pos.y + "|" + pos.z + "|" + 
                       rot.x + "|" + rot.y + "|" + rot.z + "|" + 
                       vel.x + "|" + vel.y + "|" + vel.z + "|" + 
                       xRotation + "|" + currentSpeed + "|0|0|0|0|0|0|" + 
                       Swim + "|" + transform.localScale.y + "|" + capsule.height;
                       
        PlayerPrefs.SetString("playerData" + PlayerPrefs.GetInt("WorldIndex", 0), value);
    }

    public void Revive()
    {
        if (RevivePosition != Vector3.zero) transform.position = RevivePosition;
        else transform.position = new Vector3(143, 26.5f, 210);
    }
}