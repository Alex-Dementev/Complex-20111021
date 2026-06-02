using UnityEngine;

public class Closet : MonoBehaviour, IInteractable
{
    //public TMP_Text Name;
    public int[] Slots;
    public int TotalSlots;
    public bool Spawned;
    public int ClosetType;
    public int ID;
    public InventoryPanel InventoryPanel;
    private bool OnSave;
    private bool Load;
    [HideInInspector]public string Name;
    private bool boolDestroy;
    private AllID AllID;

    private int[] resource1 = new int[2];
    private int[] resource2 = new int[2];
    private int[] resource3 = new int[2];
    private int[] resource4 = new int[2];

    private float StartDelay = 0.3f;

    

    public void UpdateState()
    {
        Load = true;

        if(!Spawned)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[ID + 12000] == 1)
            {
                Destroy(gameObject);

                return;
            }

            if(ClosetType == 0)
            {
                TotalSlots = 24;
                Slots = new int[24];
                Name = "Большой шкаф";
            }
            else if(ClosetType == 1)
            {
                TotalSlots = 12;
                Slots = new int[12];
                Name = "Маленький шкаф";
            }

            if (CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
                CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[Slots.Length];

            if (CenterSpawnedObjects.Instance.ResourcesNames[ID] != "" && CenterSpawnedObjects.Instance.ResourcesNames[ID] != null)
                Name = CenterSpawnedObjects.Instance.ResourcesNames[ID];

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
                Name = "Большой шкаф";
            }
            else if(ClosetType == 1)
            {
                Name = "Маленький шкаф";
            }

            if (CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
                CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[Slots.Length]; 

            if (CenterSpawnedObjects.Instance.ResourcesNames[ID] != "" && CenterSpawnedObjects.Instance.ResourcesNames[ID] != null)
                Name = CenterSpawnedObjects.Instance.ResourcesNames[ID];

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

            if(!OnSave && PauseController.InvisibleOperations && Load)
                UpdateCloset();
            else if(!PauseController.InvisibleOperations)
                OnSave = false;
        }

        StartDelay -= Time.deltaTime;
    }

    public void UpdateCloset()
    {
        OnSave = true;

        CenterSpawnedObjects.Instance.ResourcesNames[ID] = Name;

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
        if(boolDestroy || Time.timeScale != 1 || DestroyPreviewModels.Destroy != null || StartDelay > 0)
            return;

        boolDestroy = true;


        if(Time.timeScale == 0)
            return;

        
        for(int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i] != 0)
            {
                LiftAnObject.Instance.StartTrableAnimator("Нельзя");
                return;
            }
        }

        
        string[] Resources1 = AllID.BuildResource1[ClosetType].Split('|');
        resource1[0] = int.Parse(Resources1[0]);
        if(Resources1[1] != "")
            resource1[1] = int.Parse(Resources1[2]);
        InventorySlots.Instance.SpawnedID = 2;
        for(int i = 0; i < resource1[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources2 = AllID.BuildResource2[ClosetType].Split('|');
        resource2[0] = int.Parse(Resources2[0]);
        if(Resources2[1] != "")
            resource2[1] = int.Parse(Resources2[2]);
        InventorySlots.Instance.SpawnedID = 2;
        for(int i = 0; i < resource2[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }
        
        string[] Resources3 = AllID.BuildResource3[ClosetType].Split('|');
        resource3[0] = int.Parse(Resources3[0]);
        if(Resources3[1] != "")
            resource3[1] = int.Parse(Resources3[2]);
        InventorySlots.Instance.SpawnedID = 2;
        for(int i = 0; i < resource3[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources4 = AllID.BuildResource4[ClosetType].Split('|');
        resource4[0] = int.Parse(Resources4[0]);
        if(Resources4[1] != "")
            resource4[1] = int.Parse(Resources4[2]);
        InventorySlots.Instance.SpawnedID = 2;
        for(int i = 0; i < resource4[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
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

        Destroy(gameObject);
    }

    public string GetName()
    {
        return Name;
    }

    public void LeftClick()
    {
        if(boolDestroy || Time.timeScale != 1 || DestroyPreviewModels.Destroy != null || StartDelay > 0)
            return;
        
        InventoryPanel.Instance.OpenCloset();
        InventorySlots.Instance.Closet = this;
        InventorySlots.Instance.UpdateCloset();
        EditableClosetText.Instance.Closet = this;
        EditableClosetText.Instance.input.text = Name;
    }

    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }
}
