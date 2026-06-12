using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseController : MonoBehaviour
{
    private InputAction PauseAction;
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
                    InvisibleOperations = false;
                    PauseAnimator.Play("Close");
                    AudioManager.Instance.PauseContinue(true);

                    if(!InventoryPanel.Instance.IsActive)
                    {
                        Speed = 1;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
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

    void Start()
    {
        Speed = 1;

        var playerMap = inputActions.FindActionMap("Player");

        PauseAction = playerMap.FindAction("Pause");

        PauseAction.Enable();

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
        IdentificatorSave.color = Color.green;
    }
}
