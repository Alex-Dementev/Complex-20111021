using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction InventoryAction;
    public GameObject InventoryObject;
    public Animator InventoryAnimator;
    public Image[] ImageSlots;
    private int[] IndexSlots;
    private int CurrentSlotDetect;
    public Text Descriptions;
    public bool IsActive;
    private float IsDelay;
    private float Speed;
    public PauseController PauseController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");

        InventoryAction = playerMap.FindAction("Inventory");

        InventoryAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        IsDelay -= Time.unscaledDeltaTime;

        if(InventoryAction.IsPressed() && IsDelay <= 0)
        {
            Debug.Log("Инвентарь");
            if(IsActive)
            {
                IsActive = false;
                InventoryObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                if(!PauseController.IsActive)
                    PauseController.Speed = 1;
                Debug.Log(PauseController.IsActive);
            }
            else
            {
                IsActive = true;
                InventoryObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                PauseController.Speed = 0;
            }

            IsDelay = 0.5f;
        }
    }
}
