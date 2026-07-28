using UnityEngine;

public class ChargeButteriesBase : MonoBehaviour, IInteractable
{
    public bool Spawned;

    private int Type = 4;
    [HideInInspector] public float[] ButterySettings = new float[2];
    [HideInInspector] public int[] ButteryIndex = new int[2];
    private int CurrentSlot;
    private bool ClickToSlot;

    public int ID;

    private bool OnSave;
    private bool Load;
    private bool boolDestroy;

    [HideInInspector] public AllID AllID;

    public GameObject[] ChargeEffect1;
    public GameObject[] ChargeEffect2;
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
            CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID] = new float[2];
            CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.position;
            CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects] = transform.eulerAngles;
        }

        ButteryIndex[0] = CenterSpawnedObjects.Instance.ResourcesItems[ID][0];
        ButterySettings[0] = CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID][0];
        ButteryIndex[1] = CenterSpawnedObjects.Instance.ResourcesItems[ID][0];
        ButterySettings[1] = CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID][0];
        transform.position = CenterSpawnedObjects.Instance.ResourcesPositions[ID + CenterSpawnedObjects.IDSpawnedObjects];
        transform.eulerAngles = CenterSpawnedObjects.Instance.ResourcesRotations[ID + CenterSpawnedObjects.IDSpawnedObjects];

        Effect();
    }

    void Update()
    {
        if(AllID.Settings[ButteryIndex[0]] != 0 || AllID.Settings[ButteryIndex[1]] != 0)
        {
            ButterySettings[0] = Mathf.MoveTowards(ButterySettings[0], AllID.Settings[ButteryIndex[0]], Time.deltaTime * Speed);
            ButterySettings[1] = Mathf.MoveTowards(ButterySettings[1], AllID.Settings[ButteryIndex[1]], Time.deltaTime * Speed);
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

        CenterSpawnedObjects.Instance.ResourcesItems[ID][0] = ButteryIndex[0];
        CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID][0] = ButterySettings[0];
        CenterSpawnedObjects.Instance.ResourcesItems[ID][1] = ButteryIndex[1];
        CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID][1] = ButterySettings[1];
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
        CenterSpawnedObjects.Instance.ResourcesItemsSettings[ID] = null;

        Destroy(gameObject);
    }

    public string GetName()
    {
        return "Зарядная станция";
    }

    public void LeftClickToSlot(int LeftClickToCurrentSlot = 0) {CurrentSlot = LeftClickToCurrentSlot; ClickToSlot = true; LeftClick();}
    
    public void LeftClick()
    {
        if(boolDestroy || Time.timeScale != 1 || !Load || (DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) >= 2)
            return;

        if(DestroyPreviewModels.Destroy != null)
        {
            if(DestroyPreviewModels.Destroy.GetInvocationList().Length >= 2)
                return;
        }


        if(ClickToSlot)
        {
            ClickToSlot = false;

            if(ButteryIndex[CurrentSlot] == 0)
            {
                if(InventorySlots.Instance.IndexSlots[0] >= AllID.Backpack && InventorySlots.Instance.IndexSlots[0] < AllID.Butteries)
                {
                    ButteryIndex[CurrentSlot] = InventorySlots.Instance.IndexSlots[0];
                    ButterySettings[CurrentSlot] = InventorySlots.Instance.SettingsSlots[0];
                    InventorySlots.Instance.IndexSlots[0] = 0;
                    InventorySlots.Instance.SettingsSlots[0] = 0;

                    InventorySlots.Instance.UpplyQuickAccess(0);
                    InventorySlots.Instance.UpplySlots(0);
                    
                    Effect();
                }
                else
                    LiftAnObject.Instance.StartTrableAnimator("Этот предмет не подходит");
            }
            else
            {
                bool foundSlot = false;
                for(int i = 0; i < InventorySlots.Instance.TotalSlots; i++)
                {
                    if(InventorySlots.Instance.IndexSlots[i] == 0)
                    {
                        InventorySlots.Instance.IndexSlots[i] = ButteryIndex[CurrentSlot];
                        InventorySlots.Instance.SettingsSlots[i] = ButterySettings[CurrentSlot];
                        ButteryIndex[CurrentSlot] = 0;
                        ButterySettings[CurrentSlot] = 0;

                        if(i < 7)
                            InventorySlots.Instance.UpplyQuickAccess(i);
                        InventorySlots.Instance.UpplySlots(i);
                        
                        Effect();
                        foundSlot = true;
                        break;
                    }
                }

                if(!foundSlot)
                    LiftAnObject.Instance.StartTrableAnimator("Инвентарь полон");
            }

            return;
        }
        else if(InventorySlots.Instance.IndexSlots[0] >= AllID.Backpack && InventorySlots.Instance.IndexSlots[0] < AllID.Butteries)
        {
            if(ButteryIndex[0] == 0)
            {
                ButteryIndex[0] = InventorySlots.Instance.IndexSlots[0];
                ButterySettings[0] = InventorySlots.Instance.SettingsSlots[0];
                InventorySlots.Instance.IndexSlots[0] = 0;
                InventorySlots.Instance.SettingsSlots[0] = 0;
            }
            else if(ButteryIndex[1] == 0)
            {
                ButteryIndex[1] = InventorySlots.Instance.IndexSlots[0];
                ButterySettings[1] = InventorySlots.Instance.SettingsSlots[0];
                InventorySlots.Instance.IndexSlots[0] = 0;
                InventorySlots.Instance.SettingsSlots[0] = 0;
            }
            else
                LiftAnObject.Instance.StartTrableAnimator("Ячейки полны");

            InventorySlots.Instance.UpplyQuickAccess(0);
            InventorySlots.Instance.UpplySlots(0);
            Effect();

            return;
        }
        else
            LiftAnObject.Instance.StartTrableAnimator("Этот предмет не подходит");
    }

    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }

    private void Effect()
    {
        if(ButteryIndex[0] != 0)
        {
            if(ButterySettings[0] == AllID.Settings[ButteryIndex[0]])
            {
                ChargeEffect1[0].SetActive(true);
                ChargeEffect1[1].SetActive(false);
                ChargeEffect1[2].SetActive(false);
            }
            else
            {
                ChargeEffect1[1].SetActive(true);
                ChargeEffect1[2].SetActive(false);
                ChargeEffect1[0].SetActive(false);
            }
        }
        else
        {
            ChargeEffect1[2].SetActive(true);
            ChargeEffect1[1].SetActive(false);
            ChargeEffect1[0].SetActive(false);
        }

        if(ButteryIndex[1] != 0)
        {
            if(ButterySettings[1] == AllID.Settings[ButteryIndex[1]])
            {
                ChargeEffect2[0].SetActive(true);
                ChargeEffect2[1].SetActive(false);
                ChargeEffect2[2].SetActive(false);
            }
            else
            {
                ChargeEffect2[1].SetActive(true);
                ChargeEffect2[2].SetActive(false);
                ChargeEffect2[0].SetActive(false);
            }
        }
        else
        {
            ChargeEffect2[2].SetActive(true);
            ChargeEffect2[1].SetActive(false);
            ChargeEffect2[0].SetActive(false);
        }
    }
}
