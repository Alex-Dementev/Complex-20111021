using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class EditableClosetText : MonoBehaviour
{
    public TMP_InputField input;
    public InputActionAsset inputActions;

    private InputAction LBMAction;
    private InputAction SubmitAction;

    private bool isEditing;

    public Closet Closet;

    public static EditableClosetText Instance;
    

    void Start()
    {
        Instance = this;

        var playerMap = inputActions.FindActionMap("Player");

        LBMAction = playerMap.FindAction("ЛКМ");
        SubmitAction = playerMap.FindAction("Submit");

        LBMAction.Enable();
        SubmitAction.Enable();

        LBMAction.performed += OnLeftClick;
        SubmitAction.performed += OnSubmit;
    }

    public void StartEdit()
    {
        isEditing = true;

        input.ActivateInputField();
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!isEditing) return;

        SaveEdit();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (!isEditing) return;

        // ВАЖНО: проверяем куда кликнули
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != input.gameObject)
        {
            SaveEdit();
        }
    }

    private void SaveEdit()
    {
        if(string.IsNullOrWhiteSpace(input.text))
            input.text = Closet.Name;
        
        Closet.Name = input.text;

        isEditing = false;
    }

    void OnDestroy()
    {
        LBMAction.performed -= OnLeftClick;
        SubmitAction.performed -= OnSubmit;
    }
}
