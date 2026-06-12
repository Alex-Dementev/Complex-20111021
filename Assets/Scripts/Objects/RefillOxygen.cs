using UnityEngine;

public class RefillOxygen : MonoBehaviour, IInteractable
{
    //public TMP_Text Name;
    public bool Spawned;
    private int Type = 2;
    private int RefillOldIndex = 0;
    private int RefillIndex = 0;
    public int ID;
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
            if(CenterSpawnedObjects.Instance.ResourcesID[ID + CenterSpawnedObjects.IDSpawnedObjects] == 1)
            {
                Destroy(gameObject);

                return;
            }

            if(RefillIndex != RefillOldIndex)
            {
                RefillOldIndex = RefillIndex;
            }

            CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
            CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;
        }
        else
        {
            if(RefillIndex != RefillOldIndex)
            {
                RefillOldIndex = RefillIndex;
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

        CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
        CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;

        CenterSpawnedObjects.Instance.ResourcesTypes[ID + CenterSpawnedObjects.IDSpawnedObjects] = Type;
    }

    public void RightClick()
    {
        if(InventorySlots.Instance.IndexSlots[0] != 50 || !HandItemSpawner.ActiveItem)
        {
            LiftAnObject.Instance.StartTrableAnimator("Нужен строитель!");
            return;
        }

        if(boolDestroy || Time.timeScale != 1 || StartDelay > 0)
            return;

        if(DestroyPreviewModels.Destroy != null)
        {
            if(DestroyPreviewModels.Destroy.GetInvocationList().Length >= 2)
                return;
        }


        boolDestroy = true;

        
        string[] Resources1 = AllID.BuildResource1[Type].Split('|');
        resource1[0] = int.Parse(Resources1[0]);
        if(Resources1[1] != "")
            resource1[1] = int.Parse(Resources1[2]);
        InventorySlots.Instance.SpawnedID = resource1[0];
        for(int i = 0; i < resource1[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources2 = AllID.BuildResource2[Type].Split('|');
        resource2[0] = int.Parse(Resources2[0]);
        if(Resources2[1] != "")
            resource2[1] = int.Parse(Resources2[2]);
        InventorySlots.Instance.SpawnedID = resource2[0];
        for(int i = 0; i < resource2[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }
        
        string[] Resources3 = AllID.BuildResource3[Type].Split('|');
        resource3[0] = int.Parse(Resources3[0]);
        if(Resources3[1] != "")
            resource3[1] = int.Parse(Resources3[2]);
        InventorySlots.Instance.SpawnedID = resource3[0];
        for(int i = 0; i < resource3[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources4 = AllID.BuildResource4[Type].Split('|');
        resource4[0] = int.Parse(Resources4[0]);
        if(Resources4[1] != "")
            resource4[1] = int.Parse(Resources4[2]);
        InventorySlots.Instance.SpawnedID = resource4[0];
        for(int i = 0; i < resource4[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }


        if(Spawned)
            CenterSpawnedObjects.Instance.ResourcesID[ID + CenterSpawnedObjects.IDSpawnedObjects] = 0;
        else
            CenterSpawnedObjects.Instance.ResourcesID[ID + CenterSpawnedObjects.IDSpawnedObjects] = 1;

        CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.Instance.ResourcesTypes[ID + CenterSpawnedObjects.IDSpawnedObjects] = 0;

        CenterSpawnedObjects.Instance.ResourcesNames[ID] = null;
        CenterSpawnedObjects.Instance.ResourcesItems[ID] = null;
        CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID] = null;

        Destroy(gameObject);
    }

    public string GetName()
    {
        return Name;
    }

    public void LeftClick()
    {
        if(boolDestroy || Time.timeScale != 1 || StartDelay > 0 || (DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) >= 2)
            return;

        if(DestroyPreviewModels.Destroy != null)
        {
            if(DestroyPreviewModels.Destroy.GetInvocationList().Length >= 2)
                return;
        }


    }

    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }
}
