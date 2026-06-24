using UnityEngine;

public class LocationToRevive : MonoBehaviour
{
    public Vector3 TransportLocation;
    private bool InTrigger;
    public Transform TransformEmpty;
    public CharacterController CharacterController;
    public bool IsABase;
    public string BaseName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            InTrigger = true;

            if(IsABase)
            {
                CharacterController.RevivePosition = TransformEmpty.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            InTrigger = false;
        }
    }

    private void OnDestroy()
    {
        TransportLocation = new Vector3(0, 0, 0);
        CharacterController.RevivePosition = TransportLocation;
    }

    public void InOut()
    {
        CharacterController.RevivePosition = TransformEmpty.position;
        Debug.Log("Сохранение позиции для возрождения!");
    }
}
