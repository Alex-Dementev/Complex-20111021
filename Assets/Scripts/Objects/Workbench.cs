using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    public bool Spawned;
    public int ID;
    private bool OnSave;
    private bool Load;
    public string Name;
    private bool boolDestroy;
    private AllID AllID;

    private int[] resource1 = new int[2];
    private int[] resource2 = new int[2];
    private int[] resource3 = new int[2];
    private int[] resource4 = new int[2];

    

    public void UpdateState()
    {
        Load = true;


        if(!Spawned)
        {
            if(!Spawned && CenterSpawnedObjects.Instance.ResourcesID[ID + CenterSpawnedObjects.IDSpawnedObjects] == 1)
            {
                Destroy(gameObject);

                return;
            }

            if(CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
            {
                CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[1];
                CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
                CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;
            }

            transform.position = CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects];
            transform.eulerAngles = CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects];
        }
    }

    void Update()
    {
        if(CenterSpawnedObjects.Load)
        {
            if(!Load)
                UpdateState();

            if(!OnSave && PauseController.InvisibleOperations && Load)
                UpdateSave();
            else if(!PauseController.InvisibleOperations)
                OnSave = false;
        }
    }

    public void UpdateSave()
    {
        OnSave = true;

        CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
        CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;

        CenterSpawnedObjects.Instance.ResourcesTypes[ID + CenterSpawnedObjects.IDSpawnedObjects] = 5;
    }

    public void RightClick()
    {
        if(InventorySlots.Instance.IndexSlots[0] != 50 || !HandItemSpawner.ActiveItem)
        {
            LiftAnObject.Instance.StartTrableAnimator("Нужен строитель!");
            return;
        }

        if(boolDestroy || Time.timeScale != 1 || !Load)
            return;

        if(DestroyPreviewModels.Destroy != null)
        {
            if(DestroyPreviewModels.Destroy.GetInvocationList().Length >= 2)
                return;
        }


        if(Time.timeScale == 0)
            return;


        boolDestroy = true;

        
        string[] Resources1 = AllID.BuildResource1[5].Split('|');
        resource1[0] = int.Parse(Resources1[0]);
        if(Resources1[1] != "")
            resource1[1] = int.Parse(Resources1[2]);
        InventorySlots.Instance.SpawnedID = resource1[0];
        for(int i = 0; i < resource1[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources2 = AllID.BuildResource2[5].Split('|');
        resource2[0] = int.Parse(Resources2[0]);
        if(Resources2[1] != "")
            resource2[1] = int.Parse(Resources2[2]);
        InventorySlots.Instance.SpawnedID = resource2[0];
        for(int i = 0; i < resource2[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }
        
        string[] Resources3 = AllID.BuildResource3[5].Split('|');
        resource3[0] = int.Parse(Resources3[0]);
        if(Resources3[1] != "")
            resource3[1] = int.Parse(Resources3[2]);
        InventorySlots.Instance.SpawnedID = resource3[0];
        for(int i = 0; i < resource3[1]; i++)
        {
            InventorySlots.Instance.SpawnResourcetAfterDestroy();
        }

        string[] Resources4 = AllID.BuildResource4[5].Split('|');
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
        CenterSpawnedObjects.Instance.ResourcesItems[ID] = null;
        CenterSpawnedObjects.Instance.ResourcesTypes[ID + CenterSpawnedObjects.IDSpawnedObjects] = 0;

        Destroy(gameObject);
    }

    public string GetName()
    {
        return Name;
    }

    public bool GetLCM()
    {
        return true;
    }

    public void LeftClick()
    {
        if(boolDestroy || Time.timeScale != 1 || !Load || (DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) >= 2)
            return;

        if(!PauseController.IsActive && !InventoryPanel.Instance.IsActive)
            Crafts.Instance.Activate(0);
    }

    public bool GetDestroyBuild()
    {
        return true;
    }

    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }
}
