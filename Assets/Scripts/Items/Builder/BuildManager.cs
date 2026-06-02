using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [Header("Настройки луча")]
    public Camera playerCamera;
    public float maxBuildDistance = 8f;
    
    public InputActionAsset inputActions;
    private InputAction SpawnAction;
    public Spawn Spawn;
    public ChoiceOfBuilding ChoiceOfBuilding;

    public LayerMask buildableLayers; 

    [Header("Объекты")]
    public static Transform previewObject;

    public static PreviewMode PreviewMode;


    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        SpawnAction = playerMap.FindAction("Spawn");
        SpawnAction.Enable();
    }

    void Update()
    {
        if (previewObject == null || Time.timeScale == 0) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxBuildDistance, buildableLayers))
        {
            previewObject.gameObject.SetActive(true);

            PreviewMode.UpdatePreviewPosition(hit.point, hit.normal);

            if(SpawnAction.triggered)
            {
                Spawn.Type = ChoiceOfBuilding.Current;
                Spawn.TransformObject = previewObject.transform;

                Spawn.SpawnObject();

                DestroyPreviewModels.Destroy?.Invoke();
            }
        }
        else
        {
            previewObject.gameObject.SetActive(false);
        }
    }
}
