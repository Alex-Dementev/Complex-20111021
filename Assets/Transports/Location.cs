using UnityEngine;

public class LocationToRevive : MonoBehaviour
{
    public Vector3 TransportLocation;
    private bool InTrigger;
    public Transform TransformEmpty;
    public CharacterContorller CharacterContorller;
    public bool IsABase;
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
                CharacterContorller.RevivePosition = TransformEmpty.position;
                Debug.Log("Сохранение позиции для возрождения!");
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
        CharacterContorller.RevivePosition = TransportLocation;
    }

    public void InOut()
    {
        CharacterContorller.RevivePosition = TransformEmpty.position;
        Debug.Log("Сохранение позиции для возрождения!");
    }
}
