using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LiftAnObject : MonoBehaviour
{
    private float distance = 6f;
    private float radius = 0.05f;
    public Text NameObject;
    public InputActionAsset inputActions;
    private InputAction LiftAction;
    private InputAction DestroyAction;
    private Objects Object;
    private Closet Closet;
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
    public Text TrableText;


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

        if(LiftAction.triggered && Object != null && Closet == null && !PauseController.IsActive && !InventoryPanel.IsActive)
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
                    break;
                }
            }

            if(FullInventoryStart)
            {
                TrableText.text = "Инвентарь полон";
                FullInventoryStart = false;
                if(!FullInventoryEnd)
                    AnimatorFullInventory.CrossFade("Start", 0.2f);
                IsDelayFullInvetoryAnimator = 2;
                FullInventoryEnd = true;
            }
        }
        
        if(LiftAction.triggered && Closet != null && Object == null && !PauseController.IsActive && !InventoryPanel.IsActive)
        {
            InventoryPanel.OpenCloset();
            InventorySlots.Closet = Closet;
            InventorySlots.UpdateCloset();
            Closet = null;
        }
        if(DestroyAction.triggered && Closet != null && Object == null && !PauseController.IsActive && !InventoryPanel.IsActive)
        {
            for(int i = 0; i < Closet.Slots.Length; i++)
            {
                if(Closet.Slots[i] != 0)
                {
                    TrableText.text = "Нельзя";
                    if(!FullInventoryEnd)
                        AnimatorFullInventory.CrossFade("Start", 0.2f);
                    IsDelayFullInvetoryAnimator = 2;
                    FullInventoryEnd = true;
                    return;
                }
            }

            Closet.DestroyCloset();
            Closet = null;
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
            else if(!hit.collider.CompareTag("Closet"))
            {
                Object = null;

                if(AnimatorController)
                {
                    AnimatorController = false;
                    AnimatorNameObject.CrossFade("End", 0.2f);
                }
            }

            if (hit.collider.CompareTag("Closet"))
            {
                Closet = hit.collider.GetComponent<Closet>();

                if (Closet != null)
                {
                    NameObject.text = "Шкаф"; // + Closet.Name

                    if(!AnimatorController)
                    {
                        AnimatorController = true;
                        AnimatorNameObject.CrossFade("Start", 0.2f);
                    }
                }
            }
            else if(!hit.collider.CompareTag("Object"))
            {
                if(AnimatorController)
                {
                    AnimatorController = false;
                    AnimatorNameObject.CrossFade("End", 0.2f);
                }
            }

            if(!hit.collider.CompareTag("Closet") && !hit.collider.CompareTag("Object"))
            {
                Object = null;
                Closet = null;

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
            Closet = null;

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

        LiftAction = playerMap.FindAction("ЛКМ");
        DestroyAction = playerMap.FindAction("ПКМ");

        LiftAction.Enable();
        DestroyAction.Enable();
    }
}