using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HandItemSpawner : MonoBehaviour
{
    private int OldIndex = -1;
    private GameObject Object;
    public AllID AllID;
    public Transform HandPoint;
    public bool Active;
    public Image Slot;
    public InputActionAsset inputActions;
    private InputAction ChooseAction;
    private InputAction ThrowOutAction;
    public Animator ChangeAnimator;
    private bool ThrowOut;
    private IBeforeDestroy IBeforeDestroy;


    void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        ChooseAction = playerMap.FindAction("Choose");
        ThrowOutAction = playerMap.FindAction("ThrowOut");
        ChooseAction.Enable();
        ThrowOutAction.Enable();
    }

    void Update()
    {
        if(Time.timeScale == 0)
            return;

        
        if(InventorySlots.Instance.IndexSlots[0] != OldIndex)
        {
            ChangeAnimator.CrossFade("Change", 0.05f);
        }

        if(ChooseAction.triggered)
        {
            ChangeAnimator.CrossFade("Choose", 0.05f);
        }

        if(ThrowOutAction.triggered && Active)
        {
            ThrowOut = true;
            ChangeAnimator.CrossFade("Choose", 0.05f);
        }
    }

    public void VoidChange()
    {
        OldIndex = InventorySlots.Instance.IndexSlots[0];

        DestroyObject();

        if(InventorySlots.Instance.IndexSlots[0] <= 0)
        {
            Slot.color = new Color(55f/255f, 55f/255f, 55f/255f);
            Active = false;

            return;
        }
            
        InstantiateObject();

        Active = true;
        Slot.color = new Color(85f/255f, 85f/255f, 85f/255f);
    }

    public void VoidChooseAction()
    {
        if(ThrowOut)
        {
            Active = false;
            Slot.color = new Color(55f/255f, 55f/255f, 55f/255f);

            if(Object != null)
            {
                DestroyObject();

                InventorySlots.Instance.SpawnedID = InventorySlots.Instance.IndexSlots[0];
                InventorySlots.Instance.ThrowOut(false);
                ThrowOut = false;
            }

            return;
        }



        if(Active)
        {
            Active = false;
            Slot.color = new Color(55f/255f, 55f/255f, 55f/255f);

            DestroyObject();

            return;
        }
        else if(!Active && InventorySlots.Instance.IndexSlots[0] >= 1)
        {
            Active = true;
            Slot.color = new Color(85f/255f, 85f/255f, 85f/255f);

            InstantiateObject();
        }
    }

    private void InstantiateObject()
    {
        Object = Instantiate(AllID.HandPrefab[OldIndex]);

        Object.transform.SetParent(HandPoint, false);

        IBeforeDestroy = Object.GetComponent<IBeforeDestroy>();
    }

    private void DestroyObject()
    {
        if(Object == null)
            return;

        if(IBeforeDestroy != null)
            IBeforeDestroy.BeforeDestroy();
        Destroy(Object);
    }
}
