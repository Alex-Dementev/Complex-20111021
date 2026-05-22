using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseController : MonoBehaviour
{
    private InputAction PauseAction;
    public Animator PauseAnimator;
    [HideInInspector]public bool IsActive;
    private float IsDelay;
    public GameObject PauseObject;
    public InputActionAsset inputActions;
    [HideInInspector]public float Speed = 1f;
    public Slider SliderSensitivity;
    private float MouseSensitivity;
    public Animator BlackScreen;
    public Image IdentificatorSave;
    public AllTimeInGame AllTimeInGame;
    private float TimeSave;
    public InventoryPanel InventoryPanel;

    void Update()
    {
        IsDelay -= Time.unscaledDeltaTime;

        if (PauseAction.triggered)
        {
            if(IsDelay <= 0)
            {
                if(IsActive)
                {
                    IsActive = false;
                    PauseAnimator.Play("Close");

                    if(!InventoryPanel.IsActive)
                    {
                        Speed = 1;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
                else
                {
                    PauseObject.SetActive(true);
                    PauseAnimator.Play("Open");
                    IsActive = true;

                    Speed = 0;

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                IsDelay = 1;
            }
        }

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, Speed, Time.unscaledDeltaTime * 1.7f);


        MouseSensitivity = SliderSensitivity.value;
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity);

        if(AllTimeInGame.AllTime >= TimeSave + 15f)
            IdentificatorSave.color = Color.red;
        else
            IdentificatorSave.color = Color.green;
    }

    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");

        PauseAction = playerMap.FindAction("Pause");

        PauseAction.Enable();

        MouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        SliderSensitivity.value = MouseSensitivity;

        TimeSave = AllTimeInGame.AllTime + 10f;
    }

    public void Close()
    {
        if(IsActive)
        {
            PauseAnimator.Play("Close");

            if(!InventoryPanel.IsActive)
                Speed = 1;
            Debug.Log(InventoryPanel.IsActive);
                
            IsActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Exit()
    {
        BlackScreen.Play("End");
    }

    public void Save()
    {
        if(AllTimeInGame.AllTime >= TimeSave + 15f)
        {
            TimeSave = AllTimeInGame.AllTime;
            IdentificatorSave.color = Color.green;
        }
    }
}
