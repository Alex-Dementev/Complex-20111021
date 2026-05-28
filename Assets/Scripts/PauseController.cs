using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseController : MonoBehaviour
{
    private InputAction PauseAction;
    public Animator PauseAnimator;
    public static bool IsActive;
    private float IsDelay;
    public GameObject PauseObject;
    public InputActionAsset inputActions;
    public static float Speed = 1f;
    public Slider SliderSensitivity;
    private float MouseSensitivity;
    public Animator BlackScreen;
    public Image IdentificatorSave;
    public AllTimeInGame AllTimeInGame;
    private float TimeSave;
    public static bool InvisibleOperations;

    void Update()
    {
        var State = PauseAnimator.GetCurrentAnimatorStateInfo(0);
        if(!InvisibleOperations && State.IsName("Open") && State.normalizedTime >= 0.85f && Speed == 0.05f && IsActive)
        {
            InvisibleOperations = true;
            System.GC.Collect();
            Debug.Log("Очистка оперативной памяти и сохранение данных в массивы");
        }

        IsDelay -= Time.unscaledDeltaTime;

        if (PauseAction.triggered)
        {
            if(IsDelay <= 0)
            {
                if(IsActive)
                {
                    IsActive = false;
                    InvisibleOperations = false;
                    PauseAnimator.Play("Close");

                    if(!InventoryPanel.Instance.IsActive)
                    {
                        Speed = 1;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
                else
                {
                    PauseObject.SetActive(true);
                    IsActive = true;
                    PauseAnimator.Play("Open");

                    Speed = 0;

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                IsDelay = 1;
            }
        }

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, Speed, Time.unscaledDeltaTime * 2.5f);


        MouseSensitivity = SliderSensitivity.value;
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity);

        if(AllTimeInGame.AllTime >= TimeSave + 15f)
            IdentificatorSave.color = Color.red;
        else
            IdentificatorSave.color = Color.green;
    }

    void Start()
    {
        Speed = 1;

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

            if(!InventoryPanel.Instance.IsActive)
                Speed = 1;
                
            IsActive = false;
            InvisibleOperations = false;
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
