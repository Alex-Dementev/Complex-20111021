using UnityEngine;

public class RefillOxygen : MonoBehaviour, IInteractable
{
    public bool Spawned;
    private int Type = 2;
    private float RefillSettings = 0;
    private int RefillIndex = 0;
    public int ID;
    private bool OnSave;
    private bool Load;
    private bool boolDestroy;
    private AllID AllID;
    public GameObject[] RefillEffect;

    public float Speed = 2f;

    private int[] resource1 = new int[2];
    private int[] resource2 = new int[2];
    private int[] resource3 = new int[2];
    private int[] resource4 = new int[2];

    

    public void UpdateState()
    {
        Load = true;


        if(!Spawned && CenterSpawnedObjects.Instance.ResourcesID[ID + CenterSpawnedObjects.IDSpawnedObjects] == 1)
        {
            Destroy(gameObject);

            return;
        }

        if(CenterSpawnedObjects.Instance.ResourcesItems[ID] == null)
        {
            CenterSpawnedObjects.Instance.ResourcesItems[ID] = new int[2];
            CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
            CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;
        }

        RefillIndex = CenterSpawnedObjects.Instance.ResourcesItems[ID][0];
        RefillSettings = CenterSpawnedObjects.Instance.ResourcesItems[ID][1];
        transform.position = CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects];
        transform.eulerAngles = CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects];

        Effect();
    }

    void Update()
    {
        if(AllID.Settings[RefillIndex] != 0)
        {
            RefillSettings = Mathf.MoveTowards(RefillSettings, AllID.Settings[RefillIndex], Time.deltaTime * Speed);

            Effect();
        }

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

        CenterSpawnedObjects.Instance.ResourcesTypes[ID + CenterSpawnedObjects.IDSpawnedObjects] = Type;

        CenterSpawnedObjects.Instance.ResourcesItems[ID][0] = RefillIndex;
        CenterSpawnedObjects.Instance.ResourcesItems[ID][1] = (int)RefillSettings;
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

        CenterSpawnedObjects.Instance.ResourcesItems[ID] = null;

        Destroy(gameObject);
    }

    public string GetName()
    {
        if(RefillIndex >= AllID.Items && RefillIndex < AllID.Ballons)
            return "Кислород: " + (int)RefillSettings + "/" + AllID.Settings[RefillIndex];
        else
            return "Пусто";
    }

    public void LeftClick()
    {
        if(boolDestroy || Time.timeScale != 1 || !Load || (DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) >= 2)
            return;

        if(DestroyPreviewModels.Destroy != null)
        {
            if(DestroyPreviewModels.Destroy.GetInvocationList().Length >= 2)
                return;
        }


        if(AllID.Settings[RefillIndex] != 0)
        {
            bool foundSlot = false;
            for(int i = 0; i < InventorySlots.Instance.TotalSlots; i++)
            {
                if(InventorySlots.Instance.IndexSlots[i] == 0)
                {
                    InventorySlots.Instance.IndexSlots[i] = RefillIndex;
                    InventorySlots.Instance.SettingsSlots[i] = RefillSettings;
                    InventorySlots.Instance.UpplyQuickAccess(i);
                    InventorySlots.Instance.UpplySlots(i);
                    foundSlot = true;
                    break;
                }
            }

            if(!foundSlot)
            {
                LiftAnObject.Instance.StartTrableAnimator("Инвентарь полон");
                return;
            }

            RefillIndex = 0;
            RefillSettings = 0;
            CenterSpawnedObjects.Instance.ResourcesItems[ID][0] = 0;
            CenterSpawnedObjects.Instance.ResourcesItems[ID][1] = 0;

            Debug.Log("Ты забрал баллон с кислородом!");

            Effect();
            
            return;
        }

        if(InventorySlots.Instance.IndexSlots[0] >= AllID.Items && InventorySlots.Instance.IndexSlots[0] < AllID.Ballons)
        {
            if(AllID.Settings[RefillIndex] == 0)
            {
                RefillIndex = InventorySlots.Instance.IndexSlots[0];
                RefillSettings = InventorySlots.Instance.SettingsSlots[0];
                InventorySlots.Instance.IndexSlots[0] = 0;
                InventorySlots.Instance.SettingsSlots[0] = 0;
                CenterSpawnedObjects.Instance.ResourcesItems[ID][0] = RefillIndex;
                CenterSpawnedObjects.Instance.ResourcesItems[ID][1] = (int)RefillSettings;

                InventorySlots.Instance.UpplyQuickAccess(0);
                InventorySlots.Instance.UpplySlots(0);

                Debug.Log("Заправка кислорода!");
            }
        }
        else
        {
            LiftAnObject.Instance.StartTrableAnimator("Этот предмет не подходит!");
            Debug.Log("Этот предмет не подходит!");
        }
    }

    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }

    private void Effect()
    {
        if(RefillIndex != 0)
        {
            if(RefillSettings == AllID.Settings[RefillIndex])
            {
                RefillEffect[1].SetActive(false);
                RefillEffect[0].SetActive(true);
            }
            else
            {
                RefillEffect[1].SetActive(true);
                RefillEffect[0].SetActive(false);
            }

            RefillEffect[2].SetActive(false);
        }
        else
        {
            RefillEffect[0].SetActive(false);
            RefillEffect[1].SetActive(false);
            RefillEffect[2].SetActive(true);
        }
    }
}
