using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PickupDetector : MonoBehaviour
{
    private float distance = 6f;
    private float radius = 0.05f;
    public Text NameObject;
    public InputActionAsset inputActions;
    private InputAction LiftAction;
    private Objects Object;
    public int[] ResourcesID = new int[500];


    public void LoadResourcesID()
    {
        string data = PlayerPrefs.GetString("ResourcesID" + PlayerPrefs.GetInt("WorldIndex", 0), "");

        if (string.IsNullOrEmpty(data))
        {
            ResourcesID = new int[500];
            return;
        }

        string[] split = data.Split('|');

        for (int i = 0; i < ResourcesID.Length; i++)
        {
            if (i < split.Length)
                ResourcesID[i] = int.Parse(split[i]);
            else
                ResourcesID[i] = 0;
        }
    }

    void Update()
    {
        CheckObjects();

        // 🔥 визуализация направления
        Debug.DrawRay(transform.position, transform.forward * distance, Color.green);

        if(LiftAction.IsPressed() && Object != null)
        {
            ResourcesID[Object.ObjectID] = 1;
            Object.DestroyObject();
        }
    }

    public void Save()
    {
        string data = string.Join("|", ResourcesID);
        PlayerPrefs.SetString("ResourcesID" + PlayerPrefs.GetInt("WorldIndex", 0), data);
    }

    void CheckObjects()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Vector3 point1 = origin + Vector3.up * radius;
        Vector3 point2 = origin - Vector3.up * radius;

        RaycastHit hit;

        if (Physics.CapsuleCast(point1, point2, radius, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                Object = hit.collider.GetComponent<Objects>();

                if (Object != null)
                {
                    NameObject.text = "" + Object.Name;
                }
            }
            else
            {
                NameObject.text = "";
                Object = null;
            }
        }
        else
        {
            NameObject.text = "";
            Object = null;
        }
    }

    // 🔥 капсула в сцене (видно в Scene View)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Vector3 point1 = origin + Vector3.up * radius;
        Vector3 point2 = origin - Vector3.up * radius;

        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawWireSphere(point2, radius);
        Gizmos.DrawLine(point1, point2);

        Gizmos.DrawLine(point1, point1 + direction * distance);
        Gizmos.DrawLine(point2, point2 + direction * distance);
    }

    private void Start()
    {
        LoadResourcesID();

        var playerMap = inputActions.FindActionMap("Player");

        LiftAction = playerMap.FindAction("Lift");

        LiftAction.Enable();
    }
}