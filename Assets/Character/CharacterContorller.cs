using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(UnityEngine.CharacterController))]
public class PlayerController : MonoBehaviour
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

    [Header("Jump & Gravity")]
    public float jumpForce = 5f;
    public float jumpDelay = 0.08f;
    public float jumpCooldown = 1f;
    public float gravity = 20f;
    private float lastJumpTime = -999f;
    private float verticalVelocity = 0f;

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

    [Header("Mouse")]
    private float mouseSensitivity = 100f;
    public float maxLookAngle = 80f;
    private float tiltSpeed = 4f;
    private float currentZRotation;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform cameraPivot;
    public Animator animator;

    private UnityEngine.CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private bool isGrounded;
    public bool isSprinting;
    private bool OldSwim;

    public static Transform PlayerTransform;

    public Vector3 RevivePosition;

    private string BaseName = "Дом Гернольда";
    private bool Repetition;

    // Векторы внутренней инерции для плавания и воздуха
    private Vector3 currentMoveVelocity;

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
        PlayerTransform = transform;
        controller = GetComponent<UnityEngine.CharacterController>();

        originalScale = transform.localScale;
        if (controller != null) originalColliderHeight = controller.height;

        Cursor.lockState = CursorLockMode.Locked;

        if (PlayerPrefs.HasKey("playerData" + PlayerPrefs.GetInt("WorldIndex", 0)))
        {
            string value = PlayerPrefs.GetString("playerData" + PlayerPrefs.GetInt("WorldIndex", 0));
            string[] split = value.Split('|');

            Swim = bool.Parse(split[17]);
            controller.height = float.Parse(split[19]);
            
            controller.enabled = false; // Отключаем контроллер перед изменением позиции
            transform.position = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            transform.eulerAngles = new Vector3(float.Parse(split[3]), float.Parse(split[4]), float.Parse(split[5]));
            controller.enabled = true;

            currentMoveVelocity = new Vector3(float.Parse(split[6]), float.Parse(split[7]), float.Parse(split[8]));
            xRotation = float.Parse(split[9]);
            currentSpeed = float.Parse(split[10]);
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        string value2 = PlayerPrefs.GetString("RevivePosition" + PlayerPrefs.GetInt("WorldIndex", 0), "0|0|0");
        string[] split2 = value2.Split('|');
        RevivePosition = new Vector3(float.Parse(split2[0]), float.Parse(split2[1]), float.Parse(split2[2]));
    }

    // В CharacterController уроны от падения и столкновений обрабатываются через OnControllerColliderHit
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            Systems.Heals?.Invoke(5);
        }
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        Look();
        CheckGround();
        UpdateAnimation();
        HandleSwimState();
        HandleSwimVisual();
        ProcessMovement();

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);

        if (Swim && Time.time > waterEnterTime + waterExitLockTime)
            canExitWater = true;

        bool inWater = IsTouchingWater();

        if (inWater != Swim)
            Systems.Visibility?.Invoke(0.25f * Time.deltaTime);

        if (inWater)
        {
            Swim = true;
            if (SwimFogFS != null) SwimFogFS.SetActive(true);
            RenderSettings.fogDensity = 0.085f;
            RenderSettings.fogColor = new Color(12f / 255f, 31f / 255f, 37f / 255f);

            if (waterExitCooldownTimer > 0f)
                waterExitCooldownTimer -= Time.deltaTime;

            if (OldSwim != Swim)
            {
                OldSwim = Swim;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(0);
                    AudioManager.Instance.PlayBiomeSound(0, false);
                }
            }
        }
        else
        {
            Swim = false;
            if (SwimFogFS != null) SwimFogFS.SetActive(false);
            RenderSettings.fogDensity = 0.060f;
            RenderSettings.fogColor = new Color(0f / 255f, 0f / 255f, 0f / 255f);

            waterExitCooldownTimer = 2f;

            if (OldSwim != Swim)
            {
                OldSwim = Swim;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(1);
                    AudioManager.Instance.PlayBiomeSound(0, true);
                }
            }
        }

        if (currentSpeed >= 1f || Swim)
        {
            if (Swim)
            {
                Systems.Oxygen?.Invoke(1.3f);
                Systems.Visibility?.Invoke(0.06f * Time.deltaTime);
            }

            if (isSprinting && !Swim)
            {
                Systems.Visibility?.Invoke(0.12f * Time.deltaTime);
                Systems.Oxygen?.Invoke(1.4f);
            }
            else if (!Swim)
            {
                Systems.Visibility?.Invoke(0.06f * Time.deltaTime);
                Systems.Oxygen?.Invoke(1f);
            }
        }
        else Systems.Oxygen?.Invoke(1f);

        Systems.InAOxygen?.Invoke(InAOxygen(), BaseName, Repetition);
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

    // ---------------- ЕДИНАЯ СИСТЕМА ДВИЖЕНИЯ (CharacterController) ----------------
    void ProcessMovement()
    {
        if (applyWaterExitForce)
        {
            Vector3 forwardDirection = new Vector3(cameraPivot.forward.x, 0f, cameraPivot.forward.z).normalized;
            currentMoveVelocity = Vector3.up * waterExitForce + forwardDirection * (swimSpeed * 1.2f);
            verticalVelocity = currentMoveVelocity.y;
            applyWaterExitForce = false;
        }

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
            else swimIdleTimer += Time.deltaTime;

            if (!isMovingInput && swimIdleTimer > idleSinkDelay)
            {
                targetSwimVelocity.y = -idleSinkSpeed;
            }

            float currentHorizontalMagnitude = new Vector3(currentMoveVelocity.x, 0, currentMoveVelocity.z).magnitude;
            float targetHorizontalMagnitude = new Vector3(targetSwimVelocity.x, 0, targetSwimVelocity.z).magnitude;

            float xzAccel = targetHorizontalMagnitude < currentHorizontalMagnitude ? swimDeceleration : swimAcceleration;
            float yAccel = isMovingInput ? swimAcceleration : waterVerticalDamping;

            currentMoveVelocity.x = Mathf.MoveTowards(currentMoveVelocity.x, targetSwimVelocity.x, xzAccel * Time.deltaTime);
            currentMoveVelocity.z = Mathf.MoveTowards(currentMoveVelocity.z, targetSwimVelocity.z, xzAccel * Time.deltaTime);
            currentMoveVelocity.y = Mathf.MoveTowards(currentMoveVelocity.y, targetSwimVelocity.y, yAccel * Time.deltaTime);

            controller.Move(currentMoveVelocity * Time.deltaTime);
        }
        else
        {
            // --- ЛОГИКА НА СУШЕ / В ВОЗДУХЕ ---
            Vector3 dir = transform.right * moveInput.x + transform.forward * moveInput.y;
            dir = Vector3.ClampMagnitude(dir, 1f);

            if (isGrounded)
            {
                // Фиксация урона от падения при приземлении
                if (verticalVelocity < -8f)
                {
                    float fallSpeed = Mathf.Abs(verticalVelocity);
                    int totalDamage = 0;
                    float extraVertical = fallSpeed - 8f;
                    totalDamage += Mathf.RoundToInt(extraVertical * 3f);
                    Systems.Visibility?.Invoke(0.1f);

                    if (fallSpeed > 13f)
                    {
                        extraVertical = fallSpeed - 5f;
                        totalDamage += Mathf.RoundToInt(extraVertical * 5f);
                        Debug.Log("Высокое падение " + fallSpeed);
                        Systems.Visibility?.Invoke(0.2f);
                    }
                    else
                        Debug.Log("Падение " + fallSpeed);

                    if (totalDamage > 0)
                        Systems.Heals?.Invoke(totalDamage);
                }

                if (verticalVelocity < 0)
                    verticalVelocity = -2f; // Прижимаем к земле для корректного спуска по ступеням

                float targetSpeed = dir.magnitude > 0.1f ? (isSprinting ? moveSpeed * sprintMultiplier : moveSpeed) : 0f;
                float accel = targetSpeed > currentSpeed ? acceleration : deceleration;
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);

                Vector3 targetMove = dir * currentSpeed;
                currentMoveVelocity = new Vector3(targetMove.x, verticalVelocity, targetMove.z);
            }
            else
            {
                // Гравитация
                verticalVelocity -= gravity * Time.deltaTime;

                // Сопротивление воздуха в полете
                float airResistance = 0.5f;
                currentMoveVelocity.x = Mathf.MoveTowards(currentMoveVelocity.x, 0f, airResistance * Time.deltaTime * Mathf.Abs(currentMoveVelocity.x));
                currentMoveVelocity.z = Mathf.MoveTowards(currentMoveVelocity.z, 0f, airResistance * Time.deltaTime * Mathf.Abs(currentMoveVelocity.z));

                if (dir.magnitude > 0.1f)
                {
                    float airControlHorizontalSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
                    currentMoveVelocity.x = Mathf.MoveTowards(currentMoveVelocity.x, dir.x * airControlHorizontalSpeed, acceleration * 0.2f * Time.deltaTime);
                    currentMoveVelocity.z = Mathf.MoveTowards(currentMoveVelocity.z, dir.z * airControlHorizontalSpeed, acceleration * 0.2f * Time.deltaTime);
                }

                currentMoveVelocity.y = verticalVelocity;
            }

            controller.Move(currentMoveVelocity * Time.deltaTime);
        }
    }

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
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * 2f * gravity);
        }
    }

    void Look()
    {
        float mx = lookInput.x * mouseSensitivity * Time.deltaTime;
        float my = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= my;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        float tilt = 0;
        tiltSpeed = 4f;

        if (mx > 0)
        {
            tilt = -1;
            if (currentZRotation > 0) tiltSpeed = 15f;
        }
        else if (mx < 0)
        {
            tilt = 1;
            if (currentZRotation < 0) tiltSpeed = 15f;
        }

        if (Swim)
            currentZRotation = Mathf.MoveTowards(currentZRotation, tilt * 30f, Time.deltaTime * tiltSpeed * 1.5f);
        else
            currentZRotation = Mathf.MoveTowards(currentZRotation, tilt * 5f, Time.deltaTime * tiltSpeed);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, currentZRotation);
        transform.Rotate(Vector3.up * mx);
    }

    void CheckGround()
    {
        if (Swim) { isGrounded = false; return; }
        
        // Полагаемся на встроенную проверку CharacterController + фолбэк сферу
        isGrounded = controller.isGrounded;
        if (!isGrounded && groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;
        if (Swim) { animator.SetInteger("Speed", 0); return; }
        if (!isGrounded) { animator.SetInteger("Speed", 0); return; }
        if (currentSpeed < 2f) animator.SetInteger("Speed", 0);
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

        if (controller != null)
        {
            float h = Swim ? originalColliderHeight * swimScaleY : originalColliderHeight;
            controller.height = Mathf.Lerp(controller.height, h, Time.deltaTime * t);
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
        if (controller == null) return false;
        Vector3 point1 = transform.position + Vector3.up * (controller.height / 2f - controller.radius);
        Vector3 point2 = transform.position - Vector3.up * (controller.height / 2f - controller.radius);
        return Physics.CheckCapsule(point1, point2, controller.radius, LayerMask.GetMask("Water"));
    }

    public void Save()
    {
        Vector3 pos = transform.position; Vector3 rot = transform.eulerAngles; Vector3 vel = currentMoveVelocity;

        string value = pos.x + "|" + pos.y + "|" + pos.z + "|" +
                       rot.x + "|" + rot.y + "|" + rot.z + "|" +
                       vel.x + "|" + vel.y + "|" + vel.z + "|" +
                       xRotation + "|" + currentSpeed + "|0|0|0|0|0|0|" +
                       Swim + "|" + transform.localScale.y + "|" + controller.height;

        PlayerPrefs.SetString("playerData" + PlayerPrefs.GetInt("WorldIndex", 0), value);
    }

    public bool InAOxygen()
    {
        if (controller == null) return false;
        Vector3 point1 = transform.position + Vector3.up * (controller.height / 2f - controller.radius);
        Vector3 point2 = transform.position - Vector3.up * (controller.height / 2f - controller.radius);

        Collider[] colliders = Physics.OverlapCapsule(point1, point2, controller.radius, LayerMask.GetMask("Oxygen"));

        foreach (Collider col in colliders)
        {
            LocationToRevive locationToRevive = col.GetComponent<LocationToRevive>();

            if (locationToRevive != null)
            {
                if (BaseName == locationToRevive.BaseName) Repetition = true;
                else Repetition = false;
                BaseName = locationToRevive.BaseName;
                return true;
            }
        }

        BaseName = "";
        return false;
    }

    public void Revive()
    {
        controller.enabled = false;
        if (RevivePosition != Vector3.zero) transform.position = RevivePosition;
        else transform.position = new Vector3(143, 26.5f, 210);
        controller.enabled = true;
    }
}