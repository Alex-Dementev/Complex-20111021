using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryPanel : MonoBehaviour
{
    public static InventoryPanel Instance;
    public InputActionAsset inputActions;
    private InputAction InventoryAction;
    public GameObject InventoryObject;
    public Animator InventoryAnimator;
    [HideInInspector]public bool IsActive;
    private float IsDelay;
    private float Speed;
    public GameObject Closet;
    public InventorySlots InventorySlots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");

        InventoryAction = playerMap.FindAction("Inventory");

        InventoryAction.Enable();
    }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        IsDelay -= Time.unscaledDeltaTime;

        if(InventoryAction.triggered && IsDelay <= 0)
        {
            if(IsActive && !PauseController.IsActive)
            {
                IsActive = false;

                if(Closet.activeSelf)
                    InventoryAnimator.Play("CloseCloset");
                else
                    InventoryAnimator.Play("Close");

                InventorySlots.Closet = null;
                InventorySlots.UpdateCloset();
                PauseController.Speed = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if(!PauseController.IsActive)
            {
                Closet.SetActive(false);
                IsActive = true;
                InventoryObject.SetActive(true);
                InventoryAnimator.Play("Open");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                PauseController.Speed = 0;
            }

            IsDelay = 0.5f;
        }
    }

    public void OpenCloset()
    {
        if(IsDelay <= 0)
        {
            Closet.SetActive(true);

            if(!PauseController.IsActive)
            {
                IsActive = true;
                InventoryObject.SetActive(true);
                InventoryAnimator.Play("OpenCloset");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                PauseController.Speed = 0;
            }

            IsDelay = 0.5f;
        }
    }
}
