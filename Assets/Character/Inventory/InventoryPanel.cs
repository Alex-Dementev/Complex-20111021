using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryPanel : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction InventoryAction;
    public GameObject InventoryObject;
    public Animator InventoryAnimator;
    [HideInInspector]public bool IsActive;
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
            if(IsActive && !PauseController.IsActive)
            {
                IsActive = false;
                InventoryAnimator.Play("Close");
                PauseController.Speed = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if(!PauseController.IsActive)
            {
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
}
