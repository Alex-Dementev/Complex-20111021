using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    public GameObject goLight;

    public InputActionAsset inputActions;
    private InputAction RBMAction;

    private AllID AllID;

    private bool CanActive = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        RBMAction = playerMap.FindAction("ПКМ");
        RBMAction.Enable();

        AllID = InventorySlots.Instance.AllID;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale <= 0.1f)
            return;

        if(RBMAction.triggered)
        {
            if(CanActive)
            {
                CanActive = false;
                goLight.SetActive(false);
            }
            else if(InventorySlots.Instance.SettingsSlots[0] >= 0.1f)
            {
                CanActive = true;
                goLight.SetActive(true);
            }

            AudioManager.Instance.PlaySound(4);
        }

        if(CanActive)
        {
            Systems.Visibility?.Invoke(0.07f * Time.deltaTime);
            InventorySlots.Instance.SettingsSlots[0] -= (Time.deltaTime * 0.3f); 
            
            InventorySlots.Instance.UpplyQuickAccess(0);
            InventorySlots.Instance.UpplySlots(0);

            if(InventorySlots.Instance.SettingsSlots[0] < 0.1f)
            {
                CanActive = false;
                goLight.SetActive(false);
            }
        }
    }
}
