using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseController : MonoBehaviour
{
    private InputAction EscAction;
    public Animator PauseAnimator;
    public static bool IsActive = false;
    private float IsDelay;
    public GameObject PauseObject;
    public InputActionAsset inputActions;
    public static float Speed = 1f;
    public Slider SliderSensitivity;
    private float MouseSensitivity;
    public Animator BlackScreen;
    public Image IdentificatorSave;
    public AllTimeInGame AllTimeInGame;
    public static bool InvisibleOperations;
    public Button SaveButton;
    private float OldMouseSentensivity;
    public Animator ControlAnimator;


    void Update()
    {
        IsDelay -= Time.unscaledDeltaTime;

        if (EscAction.triggered)
        {
            if(Crafts.Instance.IsActive)
            {
                Crafts.Instance.Activate();
                return;
            }
            if(InventoryPanel.Instance.IsActive)
            {
                InventoryPanel.Instance.CloseEsc();
                return;
            }


            if(IsDelay <= 0)
            {
                if(IsActive)
                {
                    IsActive = false;
                    InvisibleOperations = false;
                    PauseAnimator.Play("Close");
                    AudioManager.Instance.PauseContinue(true);
                    Speed = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if((DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) <= 1)
                {
                    PauseObject.SetActive(true);
                    IsActive = true;
                    PauseAnimator.Play("Open");
                    SaveButton.interactable = false;
                    IdentificatorSave.color = Color.red;
                    AudioManager.Instance.PauseContinue(false);
                    Speed = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                IsDelay = 1;
            }
        }

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, Speed, Time.unscaledDeltaTime * 2.5f);

        MouseSensitivity = SliderSensitivity.value;

        if(OldMouseSentensivity != MouseSensitivity)
        {
            OldMouseSentensivity = MouseSensitivity;
            PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity);
        }

        if(IsActive)
        {
            var State = PauseAnimator.GetCurrentAnimatorStateInfo(0);
            if(!InvisibleOperations && State.IsName("Open") && State.normalizedTime >= 0.98f && Speed <= 0.02f && IsActive)
            {
                InvisibleOperations = true;
                System.GC.Collect();
                Debug.Log("Очистка оперативной памяти и сохранение данных в массивы");
                SaveButton.interactable = true;
            }
        }
    }

    public void VoidControl()
    {
        ControlAnimator.CrossFade("Open", 0.2f);
    }

    void Start()
    {
        Speed = 1;

        var playerMap = inputActions.FindActionMap("Player");

        EscAction = playerMap.FindAction("Esc");

        EscAction.Enable();

        MouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        OldMouseSentensivity = MouseSensitivity;
        SliderSensitivity.value = MouseSensitivity;
    }

    public void Close()
    {
        if(IsActive)
        {
            PauseAnimator.Play("Close");
            AudioManager.Instance.PauseContinue(false);

            if(!InventoryPanel.Instance.IsActive && !Crafts.Instance.IsActive)
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
        IdentificatorSave.color = Color.green;
    }
}
