using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PickupDetector : MonoBehaviour
{
    private float distance = 6f;
    private float radius = 0.05f;
    public Text NameObject;
    public InputActionAsset inputActions;
    private InputAction LiftAction;
    private Objects Object;
    public PauseController PauseController;
    public InventoryPanel InventoryPanel;
    public Animator AnimatorNameObject;
    public Animator AnimatorFullInventory;
    private bool AnimatorController;
    public InventorySlots InventorySlots;
    private int ObjectID;
    private bool FullInventoryStart;
    private bool FullInventoryEnd;
    private float IsDelayFullInvetoryAnimator;


    void Update()
    {
        IsDelayFullInvetoryAnimator -= Time.deltaTime;

        if(IsDelayFullInvetoryAnimator <= 0 && FullInventoryEnd)
        {
            AnimatorFullInventory.CrossFade("End", 0.2f);
            FullInventoryEnd = false;
        }

        CheckObjects();

        // 🔥 визуализация направления
        Debug.DrawRay(transform.position, transform.forward * distance, Color.green);

        if(LiftAction.IsPressed() && Object != null && !PauseController.IsActive && !InventoryPanel.IsActive)
        {
            FullInventoryStart = true;

            for(int i = 0; i < InventorySlots.TotalSlots; i++)
            {
                if(InventorySlots.IndexSlots[i] == 0 && Object != null)
                {
                    InventorySlots.IndexSlots[i] = ObjectID;
                    Object.DestroyObject();
                    Object = null;
                    FullInventoryStart = false;
                }
            }

            if(FullInventoryStart)
            {
                FullInventoryStart = false;
                if(!FullInventoryEnd)
                    AnimatorFullInventory.CrossFade("Start", 0.2f);
                IsDelayFullInvetoryAnimator = 2;
                FullInventoryEnd = true;
            }
        }
    }

    void CheckObjects()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Vector3 point1 = origin + Vector3.up * radius;
        Vector3 point2 = origin - Vector3.up * radius;

        RaycastHit hit;

        if (Physics.CapsuleCast(point1, point2, radius, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                Object = hit.collider.GetComponent<Objects>();

                if (Object != null)
                {
                    NameObject.text = "" + Object.Name;
                    ObjectID = Object.ID;

                    if(!AnimatorController)
                    {
                        AnimatorController = true;
                        AnimatorNameObject.CrossFade("Start", 0.2f);
                    }
                }
            }
            else
            {
                Object = null;

                if(AnimatorController)
                {
                    AnimatorController = false;
                    AnimatorNameObject.CrossFade("End", 0.2f);
                }
            }
        }
        else
        {
            Object = null;

            if(AnimatorController)
            {
                AnimatorController = false;
                AnimatorNameObject.CrossFade("End", 0.2f);
            }
        }
    }

    // 🔥 капсула в сцене (видно в Scene View)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Vector3 point1 = origin + Vector3.up * radius;
        Vector3 point2 = origin - Vector3.up * radius;

        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawWireSphere(point2, radius);
        Gizmos.DrawLine(point1, point2);

        Gizmos.DrawLine(point1, point1 + direction * distance);
        Gizmos.DrawLine(point2, point2 + direction * distance);
    }

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");

        LiftAction = playerMap.FindAction("Lift");

        LiftAction.Enable();
    }
}