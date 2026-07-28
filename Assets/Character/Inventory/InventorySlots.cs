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
    public Slider[] QuickAccessSliderSlots;
    public Image[] QuickAccessFillSliderSlots;
    public int TotalSlots = 5;
    public int TotalClosetSlots = 5;
    public int[] IndexSlots;
    public float[] SettingsSlots;
    public int[] ButteryIDSlots;
    public GameObject[] Slots;
    public Image[] SlotsAllocations;
    public Slider[] SlidersSlots;
    public Image[] FillSlidersSlots;
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
    public GameObject ChangeButteryObject;
    public Image ChangeButteryImage;

    private int OldIndexBackpack = 0;

    private ModuleThrowOut ModuleThrowOut;

    [HideInInspector] public bool ChangeButtery;



    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TotalSlots = 7;
        IndexSlots = new int[55];
        SettingsSlots = new float[55];
        ButteryIDSlots = new int[55];

        ModuleThrowOut = new ModuleThrowOut();

        var playerMap = inputActions.FindActionMap("Player");
        FastSendAction = playerMap.FindAction("Shift");
        ThrowOutAction = playerMap.FindAction("ThrowOut");
        FastSendAction.Enable();
        ThrowOutAction.Enable();
        

        string data = PlayerPrefs.GetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split1 = data.Split('|');
        string data2 = PlayerPrefs.GetString("InventorySettingsSlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split2 = data2.Split('|');
        string data3 = PlayerPrefs.GetString("InventoryButteryIDSlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split3 = data3.Split('|');

        for (int i = 0; i < 55; i++)
        {
            if (i < split1.Length && split1[i] != "" && split1[i] != null)
            {
                IndexSlots[i] = int.Parse(split1[i]);
            }
            else
            {
                IndexSlots[i] = 0;
            }
            if (i < split2.Length && split2[i] != "" && split2[i] != null)
            {
                SettingsSlots[i] = float.Parse(split2[i]);
            }
            else
            {
                SettingsSlots[i] = 0;
            }
            if (i < split3.Length && split3[i] != "" && split3[i] != null)
            {
                ButteryIDSlots[i] = int.Parse(split3[i]);
            }
            else
            {
                ButteryIDSlots[i] = 0;
            }

            if(i < 7)
                UpplyQuickAccess(i);
                    
            UpplySlots(i);
        }

        if(OldIndexBackpack != IndexSlots[52])
        {
            OldIndexBackpack = IndexSlots[52];
            TotalSlots = 7 + (int)AllID.Settings[IndexSlots[52]];
        }

        for(int i = 0; i < 52; i++)
        {
            if(i < TotalSlots)
                Slots[i].SetActive(true);
            else
                Slots[i].SetActive(false);
        }


        Description.text = "";
        Name.text = "";

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
            ThrowOut(false);


        if(OldIndexBackpack != IndexSlots[52])
        {
            OldIndexBackpack = IndexSlots[52];
            TotalSlots = 7 + (int)AllID.Settings[IndexSlots[52]];

            for(int i = 0; i < 28; i++)
            {
                if(i < TotalSlots)
                    Slots[i].SetActive(true);
                else
                    Slots[i].SetActive(false);
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
                    UpplySlots(i);
                }
            }

            for (int i = 0; i < Closet.Slots.Length; i++)
            {
                IndexSlots[28 + i] = Closet.Slots[i];
                SettingsSlots[28 + i] = Closet.Settings[i];
                ButteryIDSlots[28 + i] = Closet.ButteryID[i];
                UpplySlots(28 + i);
            }
        }
        else
            ClickToSlot(CurrentSlot);
    }

    public void UpplyQuickAccess(int Index)
    {
        if(QuickAccessImageSlots[Index] != null && QuickAccessImageSlots[Index].sprite != AllID.Sprites[IndexSlots[Index]])
            QuickAccessImageSlots[Index].sprite = AllID.Sprites[IndexSlots[Index]];
        if(QuickAccessSliderSlots[Index] == null)
            return;
        if(AllID.Buttery[IndexSlots[Index]])
        {
            if(ButteryIDSlots[Index] == 0 && QuickAccessSliderSlots[Index].gameObject.activeSelf)
                QuickAccessSliderSlots[Index].gameObject.SetActive(false);
            if(ButteryIDSlots[Index] != 0 && !QuickAccessSliderSlots[Index].gameObject.activeSelf)
                QuickAccessSliderSlots[Index].gameObject.SetActive(true);

            if(QuickAccessSliderSlots[Index].gameObject.activeSelf)
            {
                QuickAccessSliderSlots[Index].maxValue = AllID.Settings[ButteryIDSlots[Index]];
                QuickAccessSliderSlots[Index].value = SettingsSlots[Index];
            }

            QuickAccessFillSliderSlots[Index].color = Color.yellow; 
        }
        else
        {
            if(AllID.SliderTypes[IndexSlots[Index]] == 0 && QuickAccessSliderSlots[Index].gameObject.activeSelf)
                QuickAccessSliderSlots[Index].gameObject.SetActive(false);
            if(AllID.SliderTypes[IndexSlots[Index]] != 0 && !QuickAccessSliderSlots[Index].gameObject.activeSelf)
                QuickAccessSliderSlots[Index].gameObject.SetActive(true);

            if(QuickAccessSliderSlots[Index].gameObject.activeSelf && AllID.Settings[IndexSlots[Index]] > 12)
            {
                QuickAccessSliderSlots[Index].maxValue = AllID.Settings[IndexSlots[Index]];
                QuickAccessSliderSlots[Index].value = SettingsSlots[Index];
            }

            if(AllID.SliderTypes[IndexSlots[Index]] == 1)
                QuickAccessFillSliderSlots[Index].color = new Color(0f, 130f/255f, 170f/255f);
            else if(AllID.SliderTypes[IndexSlots[Index]] == 2)
                QuickAccessFillSliderSlots[Index].color = new Color(165f/255f, 165f/255f, 0); 
        }
    }
    public void UpplySlots(int Index)
    {
        if(ImageSlots[Index].sprite != AllID.Sprites[IndexSlots[Index]])
            ImageSlots[Index].sprite = AllID.Sprites[IndexSlots[Index]];
        if(SlidersSlots[Index] == null)
            return;
        if(AllID.Buttery[IndexSlots[Index]])
        {
            if(ButteryIDSlots[Index] == 0 && SlidersSlots[Index].gameObject.activeSelf)
                SlidersSlots[Index].gameObject.SetActive(false);
            if(ButteryIDSlots[Index] != 0 && !SlidersSlots[Index].gameObject.activeSelf)
                SlidersSlots[Index].gameObject.SetActive(true);

            if(SlidersSlots[Index].gameObject.activeSelf)
            {
                SlidersSlots[Index].maxValue = AllID.Settings[ButteryIDSlots[Index]];
                SlidersSlots[Index].value = SettingsSlots[Index];
            }

            FillSlidersSlots[Index].color = new Color(165f/255f, 165f/255f, 0); 
        }
        else
        {
            if(AllID.SliderTypes[IndexSlots[Index]] == 0 && SlidersSlots[Index].gameObject.activeSelf)
                SlidersSlots[Index].gameObject.SetActive(false);
            if(AllID.SliderTypes[IndexSlots[Index]] != 0 && !SlidersSlots[Index].gameObject.activeSelf)
                SlidersSlots[Index].gameObject.SetActive(true);

            if(SlidersSlots[Index].gameObject.activeSelf && AllID.Settings[IndexSlots[Index]] > 12)
            {
                SlidersSlots[Index].maxValue = AllID.Settings[IndexSlots[Index]];
                SlidersSlots[Index].value = SettingsSlots[Index];
            }

            if(AllID.SliderTypes[IndexSlots[Index]] == 1)
                FillSlidersSlots[Index].color = new Color(0, 130f/255f, 170f/255f);
            else if(AllID.SliderTypes[IndexSlots[Index]] == 2)
                FillSlidersSlots[Index].color = new Color(165f/255f, 165f/255f, 0); 
        }
    }

    public void ClickToSlot(int Index)
    {
        ChangeButteryObject.SetActive(false);

        if(!PauseController.IsActive && !InventoryPanel.Instance.IsActive && Index < -1)
        {
            Index += 1;
            Index = Index * -1;
            int tempID = IndexSlots[0];
            IndexSlots[0] = IndexSlots[Index];
            IndexSlots[Index] = tempID;
            float tempIDfloat = SettingsSlots[0];
            SettingsSlots[0] = SettingsSlots[Index];
            SettingsSlots[Index] = tempIDfloat;
            int tempButteryID = ButteryIDSlots[0];
            ButteryIDSlots[0] = ButteryIDSlots[Index];
            ButteryIDSlots[Index] = tempButteryID;

            UpplyQuickAccess(0);
            UpplyQuickAccess(Index);
            UpplySlots(0);
            UpplySlots(Index);

            return;
        }

        if(ChangeButtery)
        {
            ChangeButtery = false;
            ChangeButteryImage.color = new Color(73f/255f, 73f/255f, 73f/255f);

            if(ButteryIDSlots[PreviousSlot] == 0 && Index >= 0 && IndexSlots[Index] >= AllID.Backpack && IndexSlots[Index] < AllID.Butteries)
            {
                ButteryIDSlots[PreviousSlot] = IndexSlots[Index];
                SettingsSlots[PreviousSlot] = SettingsSlots[Index];
                IndexSlots[Index] = 0;
                SettingsSlots[Index] = 0;
            }
            else if(ButteryIDSlots[PreviousSlot] != 0 && Index >= 0 && IndexSlots[Index] >= AllID.Backpack && IndexSlots[Index] < AllID.Butteries || IndexSlots[Index] == 0)
            {
                int tempID = IndexSlots[Index];
                float tempSettings = SettingsSlots[Index];
                IndexSlots[Index] = ButteryIDSlots[PreviousSlot];
                SettingsSlots[Index] = SettingsSlots[PreviousSlot];
                ButteryIDSlots[PreviousSlot] = tempID;
                SettingsSlots[PreviousSlot] = tempSettings;
            }

            if(PreviousSlot < 7)
                UpplyQuickAccess(PreviousSlot);
            if(Index < 7)
                UpplyQuickAccess(Index);
            UpplySlots(PreviousSlot);
            UpplySlots(Index);

            UnSelect();

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
                        ButteryIDSlots[i] = ButteryIDSlots[Index];
                        ButteryIDSlots[Index] = 0;
                        Closet.Slots[i - 28] = IndexSlots[i];
                        Closet.Settings[i - 28] = SettingsSlots[i];
                        Closet.ButteryID[i - 28] = ButteryIDSlots[i];
                        Closet.UpdateSave();

                        if(i < 7)
                            UpplyQuickAccess(i);
                        if(Index < 7)
                            UpplyQuickAccess(Index);
                        UpplySlots(i);
                        UpplySlots(Index);

                        return;
                    }
                }
            }
            else if(Index >= 28 && Equipment(Index))
            {
                for(int i = 0; i < TotalSlots; i++)
                {
                    if(IndexSlots[i] == 0)
                    {
                        IndexSlots[i] = IndexSlots[Index];
                        IndexSlots[Index] = 0;
                        SettingsSlots[i] = SettingsSlots[Index];
                        SettingsSlots[Index] = 0;
                        ButteryIDSlots[i] = ButteryIDSlots[Index];
                        ButteryIDSlots[Index] = 0;
                        Closet.Slots[Index - 28] = 0;
                        Closet.Settings[Index - 28] = 0;
                        Closet.ButteryID[Index - 28] = 0;
                        Closet.UpdateSave();

                        if(i < 7)
                            UpplyQuickAccess(i);
                        if(Index < 7)
                            UpplyQuickAccess(Index);
                        UpplySlots(i);
                        UpplySlots(Index);

                        return;
                    }
                }
            }
        }

        if(PreviousSlot != -1)
            SlotsAllocations[PreviousSlot].color = new Color(55f/255f, 55f/255f, 55f/255f);

        if (CurrentSlot != -1 && Index != PreviousSlot && Equipment(Index))
        {
            int tempID = IndexSlots[PreviousSlot];
            IndexSlots[PreviousSlot] = IndexSlots[Index];
            IndexSlots[Index] = tempID;
            float tempIDfloat = SettingsSlots[PreviousSlot];
            SettingsSlots[PreviousSlot] = SettingsSlots[Index];
            SettingsSlots[Index] = tempIDfloat;
            int tempButteryID = ButteryIDSlots[PreviousSlot];
            ButteryIDSlots[PreviousSlot] = ButteryIDSlots[Index];
            ButteryIDSlots[Index] = tempButteryID;

            SlotsAllocations[CurrentSlot].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);
            SlotsAllocations[Index].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);

            if(PreviousSlot < 7)
                UpplyQuickAccess(PreviousSlot);
            if(Index < 7)
                UpplyQuickAccess(Index);
            UpplySlots(PreviousSlot);
            UpplySlots(Index);

            UnSelect();

            if(Closet != null)
            {
                for (int i = 0; i < Closet.TotalSlots; i++)
                {
                    Closet.Slots[i] = IndexSlots[28 + i];
                    Closet.Settings[i] = SettingsSlots[28 + i];
                    Closet.ButteryID[i] = ButteryIDSlots[28 + i];
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

                if(AllID.Settings[ButteryIDSlots[Index]] != 0)
                    Description.text = AllID.Descriptions[IndexSlots[Index]] + ".\n" + AllID.SettingsStrings[ButteryIDSlots[Index]] + ": " + (int)SettingsSlots[Index] + "/" + AllID.Settings[ButteryIDSlots[Index]];
                else if(AllID.Settings[IndexSlots[Index]] != 0 && AllID.Settings[IndexSlots[Index]] >= 13)
                    Description.text = AllID.Descriptions[IndexSlots[Index]] + ".\n" + AllID.SettingsStrings[IndexSlots[Index]] + ": " + (int)SettingsSlots[Index] + "/" + AllID.Settings[IndexSlots[Index]];
                else if(AllID.Settings[IndexSlots[Index]] > 0f)
                {
                    Description.text = AllID.Descriptions[IndexSlots[Index]] + ".\n" + AllID.SettingsStrings[IndexSlots[Index]] + ": " + SettingsSlots[Index];
                }
                else
                    Description.text = AllID.Descriptions[IndexSlots[Index]];

                Name.text = AllID.Names[IndexSlots[Index]];

                if(AllID.Buttery[IndexSlots[Index]])
                    ChangeButteryObject.SetActive(true);
                else
                    ChangeButteryObject.SetActive(false);

                CurrentSlot = Index;
                PreviousSlot = Index;
            }
            else
                CurrentSlot = -1;
        }

        if(Closet != null)
            Closet.UpdateSave();
    }

    public void VoidChangeButtery()
    {
        ChangeButtery = !ChangeButtery;

        if(ChangeButtery)
            ChangeButteryImage.color = new Color(9f/255f, 77f/255f, 0f/255f);
        else
            ChangeButteryImage.color = new Color(73f/255f, 73f/255f, 73f/255f);
    }

    private void UnSelect()
    {
        ChangeButteryObject.SetActive(false);

        if(CurrentSlot != -1)
        {
            SlotsAllocations[CurrentSlot].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);
            CurrentSlot = -1;
        }

        if(PreviousSlot != -1)
        {
            SlotsAllocations[PreviousSlot].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);
            PreviousSlot = -1;
        }

        Description.text = "";
        Name.text = "";
    }
    private bool Equipment(int Index = 0)
    {
        if(Index == 52 || PreviousSlot == 52)
        {
            for(int i = 27; i > 6; i--)
            {
                if(IndexSlots[i] != 0)
                {
                    UnSelect();

                    return false;
                }
            }

            if(PreviousSlot != -1)
            {
                if(IndexSlots[Index] != 0)
                {
                    if(IndexSlots[Index] < AllID.Ballons || IndexSlots[Index] > AllID.Backpack)
                    {
                        UnSelect();

                        return false;
                    }
                }
                else if(IndexSlots[PreviousSlot] != 0)
                {
                    if(IndexSlots[PreviousSlot] < AllID.Ballons || IndexSlots[PreviousSlot] > AllID.Backpack)
                    {
                        UnSelect();

                        return false;
                    }
                }
            }
        }
        if(Index >= 53 || PreviousSlot >= 53)
        {
            if(PreviousSlot != -1)
            {
                if(IndexSlots[Index] != 0)
                {
                    if(IndexSlots[Index] < AllID.Items || IndexSlots[Index] > AllID.Ballons)
                    {
                        UnSelect();
                        
                        return false;
                    }
                }
                else if(IndexSlots[PreviousSlot] != 0)
                {
                    if(IndexSlots[PreviousSlot] < AllID.Items || IndexSlots[PreviousSlot] > AllID.Ballons)
                    {
                        UnSelect();
                        
                        return false;
                    }
                }
            }
        }
        return true;
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
        string data2 = string.Join("|", SettingsSlots);
        PlayerPrefs.SetString("InventorySettingsSlots" + PlayerPrefs.GetInt("WorldIndex", 0), data2);
        string data3 = string.Join("|", ButteryIDSlots);
        PlayerPrefs.SetString("InventoryButteryIDSlots" + PlayerPrefs.GetInt("WorldIndex", 0), data3);
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

                IndexSlots[d] = 0;
                SettingsSlots[d] = 0;
                ImageSlots[d].color = new Color(0, 0, 0, 0f);
                UpplySlots(d);
                if(d < 7)
                    UpplyQuickAccess(d);

                if(Closet != null && CurrentSlot != -1)
                    Closet.Slots[CurrentSlot] = 0;

                Description.text = "";
                Name.text = "";
            }
        }
    }
}
