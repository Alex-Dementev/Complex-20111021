using UnityEngine;

public class Closet : MonoBehaviour, IInteractable
{
    //public TMP_Text Name;
    public int[] Slots;
    public int TotalSlots;
    public bool Spawned;
    public int ClosetType;
    public int ID;
    private int Price;
    public InventoryPanel InventoryPanel;
    private bool OnSave;
    private bool Load;

    

    void Start()
    {
        Invoke("UpdateState", 0.2f);
    }

    public void UpdateState()
    {
        Load = true;

        if(!Spawned)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[ID + 12000] == 1)
            {
                Destroy(gameObject);
            }

            if(ClosetType == 0)
            {
                TotalSlots = 24;
                Slots = new int[24];
                Price = 2;
            }
            else if(ClosetType == 1)
            {
                TotalSlots = 12;
                Slots = new int[12];
                Price = 1;
            }

            if (CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
                CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[Slots.Length];

            for(int i = 0; i < TotalSlots; i++)
            {
                Slots[i] = CenterSpawnedObjects.Instance.ResourcesItems[ID][i];
            }

            CenterSpawnedObjects.Instance.ResourcesPositions[ID + 12000] = transform.position;
            CenterSpawnedObjects.Instance.ResourcesRotations[ID + 12000] = transform.eulerAngles;
        }
        else
        {
            if(ClosetType == 0)
            {
                TotalSlots = 24;
                Price = 2;
            }
            else if(ClosetType == 1)
            {
                TotalSlots = 12;
                Price = 1;
            }

            if (CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
                CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[Slots.Length]; 

            for(int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = CenterSpawnedObjects.Instance.ResourcesItems[ID][i];
            }
        }
    }

    void Update()
    {
        if(CenterSpawnedObjects.Load)
        {
            if(!Load)
                UpdateState();

            if(!OnSave && PauseController.OpenPause && Load)
                UpdateCloset();
            else if(!PauseController.OpenPause)
                OnSave = false;
        }
    }

    public void UpdateCloset()
    {
        OnSave = true;

        CenterSpawnedObjects.Instance.ResourcesPositions[ID + 12000] = transform.position;
        CenterSpawnedObjects.Instance.ResourcesRotations[ID + 12000] = transform.eulerAngles;

        CenterSpawnedObjects.Instance.ResourcesTypes[ID + 12000] = ClosetType;

        if (CenterSpawnedObjects.Instance.ResourcesItems[ID] == null || CenterSpawnedObjects.Instance.ResourcesItems[ID].Length != Slots.Length)
        {
            CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[Slots.Length]; 
        }
        
        for(int i = 0; i < Slots.Length; i++)
        {
            CenterSpawnedObjects.Instance.ResourcesItems[ID][i] = Slots[i];
        }
    }

    public void RightClick()
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i] != 0)
            {
                LiftAnObject.Instance.StartTrableAnimator("Нельзя");
                return;
            }
        }

        if(Spawned)
            CenterSpawnedObjects.Instance.ResourcesID[ID + 12000] = 0;
        else
            CenterSpawnedObjects.Instance.ResourcesID[ID + 12000] = 1;

        CenterSpawnedObjects.Instance.ResourcesPositions[ID + 12000] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.Instance.ResourcesRotations[ID + 12000] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.Instance.ResourcesTypes[ID + 12000] = 0;

        CenterSpawnedObjects.Instance.ResourcesNames[ID] = null;
        CenterSpawnedObjects.Instance.ResourcesItems[ID] = null;

        InventorySlots.Instance.SpawnedID = 2;

        for(int i = 0; i < Price; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        Destroy(gameObject);
    }

    public string GetName()
    {
        return "Шкаф";
    }

    public void LeftClick()
    {
        InventoryPanel.Instance.OpenCloset();
        InventorySlots.Instance.Closet = this;
        InventorySlots.Instance.UpdateCloset();
    }
}
