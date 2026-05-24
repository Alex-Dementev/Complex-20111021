using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LiftAnObject : MonoBehaviour
{
    public static LiftAnObject Instance;
    private float distance = 6f;
    private float radius = 0.05f;
    public Text NameObject;
    public InputActionAsset inputActions;
    private InputAction LiftAction;
    private InputAction DestroyAction;
    public PauseController PauseController;
    public InventoryPanel InventoryPanel;
    public Animator AnimatorNameObject;
    public Animator TrableAnimator;
    private bool AnimatorController;
    private bool FullInventoryStart;
    private bool TrableAnimateEnd;
    private float IsDelayTrableAnimator;
    public Text TrableText;
    private IInteractable IInteractable;


    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        IsDelayTrableAnimator -= Time.deltaTime;

        if(IsDelayTrableAnimator <= 0 && TrableAnimateEnd)
        {
            TrableAnimator.CrossFade("End", 0.2f);
            TrableAnimateEnd = false;
        }

        CheckObjects();

        // 🔥 визуализация направления
        Debug.DrawRay(transform.position, transform.forward * distance, Color.green);

        
        if(LiftAction.triggered && IInteractable != null && !PauseController.IsActive && !InventoryPanel.IsActive)
        {
            IInteractable.LeftClick();
        }
        if(DestroyAction.triggered && IInteractable != null && !PauseController.IsActive && !InventoryPanel.IsActive)
        {
            IInteractable.RightClick();
        }
    }
    
    public void StartTrableAnimator(string Text)
    {
        TrableText.text = Text;
        if(!TrableAnimateEnd)
            TrableAnimator.CrossFade("Start", 0.2f);
        IsDelayTrableAnimator = 2;
        TrableAnimateEnd = true;
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
            IInteractable = hit.collider.GetComponent<IInteractable>();

            if (IInteractable != null)
            {
                NameObject.text = IInteractable.GetName();

                if(!AnimatorController)
                {
                    AnimatorController = true;
                    AnimatorNameObject.CrossFade("Start", 0.2f);
                }
            }
            else
            {
                IInteractable = null;

                if(AnimatorController)
                {
                    AnimatorController = false;
                    AnimatorNameObject.CrossFade("End", 0.2f);
                }
            }
        }
        else
        {
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