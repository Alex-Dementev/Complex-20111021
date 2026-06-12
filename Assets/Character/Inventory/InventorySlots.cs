using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventorySlots : MonoBehaviour
{
    public static InventorySlots Instance;
    public InputActionAsset inputActions;
    private InputAction FastSendAction;
    private InputAction ThrowOutAction;
    private InputAction[] QuickSlots = new InputAction[7];
    public Image[] ImageSlots;
    public Image[] QuickAccessImageSlots;
    public int TotalSlots = 5;
    public int TotalClosetSlots = 5;
    public int[] IndexSlots;
    public float[] SettingsSlots;
    public GameObject[] Slots;
    public Image[] SlotsAllocations;
    public Text Name;
    public Text Description;
    public AllID AllID;
    public int CurrentSlot = -1;
    private int PreviousSlot = -1;
    [HideInInspector] public int[] ClosetSlots;
    [HideInInspector] public Closet Closet;

    [Header("Ссылки для спавна")]
    public Transform SpawnPos;
    public int SpawnedID;

    private ModuleThrowOut ModuleThrowOut;



    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TotalSlots = 7;
        IndexSlots = new int[55];
        SettingsSlots = new float[55];

        ModuleThrowOut = new ModuleThrowOut();

        for(int i = 0; i < 52; i++)
        {
            if(i <= TotalSlots - 1)
                Slots[i].SetActive(true);
            else
                Slots[i].SetActive(false);
        }
        

        string data = PlayerPrefs.GetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split1 = data.Split('|');
        data = PlayerPrefs.GetString("InventorySettingsSlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split2 = data.Split('|');

        for (int i = 0; i < 55; i++)
        {
            if (i < split1.Length && split1[i] != "")
            {
                IndexSlots[i] = int.Parse(split1[i]);
                SettingsSlots[i] = int.Parse(split2[i]);
            }
            else
            {
                IndexSlots[i] = 0;
                SettingsSlots[i] = 0;
            }
        }


        Description.text = "";
        Name.text = "";


        var playerMap = inputActions.FindActionMap("Player");
        FastSendAction = playerMap.FindAction("Shift");
        ThrowOutAction = playerMap.FindAction("ThrowOut");
        FastSendAction.Enable();
        ThrowOutAction.Enable();

        for(int i = 0; i < QuickSlots.Length; i++)
        {
            if(i != 0)
            {
                QuickSlots[i] = playerMap.FindAction("QuickSlot" + (i + 1));
                QuickSlots[i].Enable();
            }
        }
    }

    void Update()
    {
        if(ThrowOutAction.triggered)
            ThrowOut();

        for(int i = 0; i < 55; i++)
        {
            if(ImageSlots[i] != null && ImageSlots[i].sprite != AllID.Sprites[IndexSlots[i]])
            {
                ImageSlots[i].sprite = AllID.Sprites[IndexSlots[i]];
            }
        }

        for(int i = 0; i < QuickAccessImageSlots.Length; i++)
        {
            if(QuickAccessImageSlots[i] != null && QuickAccessImageSlots[i].sprite != AllID.Sprites[IndexSlots[i]])
            {
                QuickAccessImageSlots[i].sprite = AllID.Sprites[IndexSlots[i]];
            }
        }

        for(int i = 1; i < QuickSlots.Length; i++)
        {
            if(QuickSlots[i].triggered)
            {
                i += 1;
                i = i * -1;
                ClickToSlot(i);
                return;
            }
        }
    }

    public void UpdateCloset()
    {
        if(Closet != null)
        {
            TotalClosetSlots = Closet.TotalSlots;
            

            for (int i = 28; i < 52; i++)
            {
                if(i <= (TotalClosetSlots + 27))
                {
                    Slots[i].SetActive(true);
                }
                else
                {
                    Slots[i].SetActive(false);
                }
            }

            for (int i = 0; i < Closet.Slots.Length; i++)
            {
                IndexSlots[28 + i] = Closet.Slots[i];
                SettingsSlots[28 + i] = Closet.Settings[i];
            }
        }
        else
            ClickToSlot(CurrentSlot);
    }

    public void ClickToSlot(int Index)
    {
        if(!PauseController.IsActive && !InventoryPanel.Instance.IsActive && Index <= -2)
        {
            Index += 1;
            Index = Index * -1;
            int tempID = IndexSlots[0];
            IndexSlots[0] = IndexSlots[Index];
            IndexSlots[Index] = tempID;
            float tempIDfloat = SettingsSlots[0];
            SettingsSlots[0] = SettingsSlots[Index];
            SettingsSlots[Index] = tempIDfloat;

            return;
        }

        if(Closet != null && FastSendAction.IsPressed())
        {
            if(Index <= TotalSlots)
            {
                for(int i = 28; i < (TotalClosetSlots + 28); i++)
                {
                    if(IndexSlots[i] == 0)
                    {
                        IndexSlots[i] = IndexSlots[Index];
                        IndexSlots[Index] = 0;
                        SettingsSlots[i] = SettingsSlots[Index];
                        SettingsSlots[Index] = 0;
                        Closet.Slots[i - 28] = IndexSlots[i];
                        Closet.Settings[i - 28] = SettingsSlots[i];
                        Closet.UpdateCloset();

                        return;
                    }
                }
            }
            else if(Index >= 28)
            {
                for(int i = 0; i < TotalSlots; i++)
                {
                    if(IndexSlots[i] == 0)
                    {
                        IndexSlots[i] = IndexSlots[Index];
                        IndexSlots[Index] = 0;
                        SettingsSlots[i] = SettingsSlots[Index];
                        SettingsSlots[Index] = 0;
                        Closet.Slots[Index - 28] = 0;
                        Closet.UpdateCloset();

                        return;
                    }
                }
            }
        }

        if(PreviousSlot != -1)
            SlotsAllocations[PreviousSlot].color = new Color(55f/255f, 55f/255f, 55f/255f);

        if (CurrentSlot != -1 && Index != PreviousSlot)
        {
            if(Index == 52 || PreviousSlot == 52)
            {
                if(IndexSlots[Index] != 0)
                {
                    if(IndexSlots[Index] < AllID.Ballons || IndexSlots[Index] > AllID.Backpack)
                    {
                        UnSelect();

                        return;
                    }
                }
                else if(PreviousSlot != 0)
                {
                    if(IndexSlots[PreviousSlot] < AllID.Ballons || IndexSlots[PreviousSlot] > AllID.Backpack)
                    {
                        UnSelect();
                        
                        return;
                    }
                }
            }
            if(Index >= 53 || PreviousSlot >= 53)
            {
                if(IndexSlots[Index] != 0)
                {
                    if(IndexSlots[Index] < AllID.Items || IndexSlots[Index] > AllID.Ballons)
                    {
                        UnSelect();
                        
                        return;
                    }
                }
                else if(PreviousSlot != 0)
                {
                    if(IndexSlots[PreviousSlot] < AllID.Items || IndexSlots[PreviousSlot] > AllID.Ballons)
                    {
                        UnSelect();
                        
                        return;
                    }
                }
            }

            int tempID = IndexSlots[PreviousSlot];
            IndexSlots[PreviousSlot] = IndexSlots[Index];
            IndexSlots[Index] = tempID;
            float tempIDfloat = SettingsSlots[PreviousSlot];
            SettingsSlots[PreviousSlot] = SettingsSlots[Index];
            SettingsSlots[Index] = tempIDfloat;

            SlotsAllocations[CurrentSlot].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);
            SlotsAllocations[Index].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);

            UnSelect();

            if(Closet != null)
            {
                for (int i = 0; i < Closet.TotalSlots; i++)
                {
                    Closet.Slots[i] = IndexSlots[28 + i];
                    Closet.Settings[i] = SettingsSlots[28 + i];
                }
            }
        }
        else
        {
            Description.text = "";
            Name.text = "";

            if(Index != CurrentSlot && Index >= 0)
            {
                SlotsAllocations[Index].color = new Color(85f/255f, 85f/255f, 85f/255f);

                if(AllID.Settings[IndexSlots[Index]] != 0)
                    Description.text = AllID.Descriptions[IndexSlots[Index]] + ".\n" + AllID.SettingsStrings[IndexSlots[Index]] + ": " + (int)SettingsSlots[Index] + "/" + AllID.Settings[IndexSlots[Index]];
                else
                    Description.text = AllID.Descriptions[IndexSlots[Index]];

                Name.text = AllID.Names[IndexSlots[Index]];

                CurrentSlot = Index;
                PreviousSlot = Index;
            }
            else
                CurrentSlot = -1;
        }

        if(Closet != null)
            Closet.UpdateCloset();
    }

    public void UnSelect()
    {
        {
            CurrentSlot = -1;
            PreviousSlot = -1;

            Description.text = "";
            Name.text = "";
        }
    }

    public void ThrowOut(bool InInventory = true)
    {
        ModuleThrowOut.ThrowOut(InInventory);
    }

    public void SpawnResourcetAfterDestroy()
    {
        ModuleThrowOut.SpawnResourcetAfterDestroy();
    }

    public void Save()
    {
        string data = string.Join("|", IndexSlots);
        PlayerPrefs.SetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), data);
        data = string.Join("|", SettingsSlots);
        PlayerPrefs.SetString("InventorySettingsSlots" + PlayerPrefs.GetInt("WorldIndex", 0), data);
    }

    public void OnDeath()
    {
        for(int d = 0; d < TotalSlots; d++)
        {
            if(IndexSlots[d] != 0 && IndexSlots[d] < AllID.Resources)
            {
                Vector3 pos = new Vector3(
                SpawnPos.position.x + Random.Range(-0.4f, 0.4f),
                SpawnPos.position.y - 0.1f,
                SpawnPos.position.z + Random.Range(-0.4f, 0.4f));

                int ObjectID = -1;

                for(int i = CenterSpawnedObjects.IDNotSpawnedObjects; i < CenterSpawnedObjects.IDSpawnedObjects; i++)
                {
                    if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
                    {
                        ObjectID = i;
                        break;
                    }
                }

                Objects obj = Instantiate(AllID.Prefab[IndexSlots[d]], pos, Quaternion.identity);

                CenterSpawnedObjects.Instance.ResourcesID[ObjectID] = 1;
                CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID] = pos;
                CenterSpawnedObjects.Instance.ResourcesTypes[ObjectID] = IndexSlots[d];

                obj.ObjectID = ObjectID;
                obj.Spawned = true;
                obj.ID = IndexSlots[d];
                obj.AllID = AllID;

                IndexSlots[d] = 0;
                SettingsSlots[d] = 0;
                ImageSlots[d].color = new Color(0, 0, 0, 0f);

                if(Closet != null && CurrentSlot != -1)
                    Closet.Slots[CurrentSlot] = 0;

                Description.text = "";
                Name.text = "";
            }
        }
    }
}
