using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    public GameObject goLight;

    public InputActionAsset inputActions;
    private InputAction RBMAction;

    private bool CanActive = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        RBMAction = playerMap.FindAction("ПКМ");
        RBMAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(RBMAction.triggered)
        {
            if(CanActive)
            {
                CanActive = false;
                goLight.SetActive(false);
            }
            else
            {
                CanActive = true;
                goLight.SetActive(true);
            }

            AudioManager.Instance.PlaySound(4);
        }

        if(CanActive)
            Systems.Visibility?.Invoke(0.07f * Time.deltaTime);
    }
}
