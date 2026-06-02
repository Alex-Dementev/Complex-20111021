using UnityEngine;
using UnityEngine.InputSystem;

public class PreviewMode : MonoBehaviour
{
    private float maxSlopeAngle = 45f;

    public LayerMask buildBlockMask;


    [Header("Размер объекта")]
    public Vector3 objectSize = new Vector3(1f, 1f, 1f);


    [Header("Размер куба проверки столкновений")]
    public Vector3 checkBoxSize = new Vector3(1f, 1f, 1f);


    public InputActionAsset inputActions;
    private InputAction scrollMouseAction;

    private Quaternion slopeRot;
    private Quaternion customRot;
    private float currentRotationY;
    private Vector2 scroll;

    public bool isValidPlace;



    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        scrollMouseAction = playerMap.FindAction("ScrollMouse");
        scrollMouseAction.Enable();

        DestroyPreviewModels.Destroy += VoidDestroy;
    }

    private void VoidDestroy()
    {
        DestroyPreviewModels.Destroy -= VoidDestroy;

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        scroll = scrollMouseAction.ReadValue<Vector2>();
        currentRotationY += (scroll.y * 3f);
        customRot = Quaternion.AngleAxis(currentRotationY, Vector3.up);

        transform.rotation = slopeRot * customRot;
    }

    public void UpdatePreviewPosition(Vector3 hitPoint, Vector3 surfaceNormal)
    {
        transform.position = hitPoint + transform.up * objectSize.y;
        

        float slopeAngle = Vector3.Angle(Vector3.up, surfaceNormal);

        if (slopeAngle > maxSlopeAngle)
        {
            SetInvalid();
            return;
        }

        slopeRot = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

        transform.rotation = slopeRot * customRot;


        bool blocked = Physics.CheckBox(transform.position + transform.up * (checkBoxSize.y * 0.02f), checkBoxSize * 0.5f, transform.rotation, buildBlockMask);

        if (blocked)
        {
            SetInvalid();
            return;
        }

        SetValid();
    }

    private void SetValid()
    {
        isValidPlace = true;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    private void SetInvalid()
    {
        isValidPlace = false;

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!gameObject) return;

        Gizmos.color = isValidPlace ? Color.green : Color.red;

        Vector3 center = transform.position + transform.up * (checkBoxSize.y * 0.02f);

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, checkBoxSize);
    }
}