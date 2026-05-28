using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventorySlots : MonoBehaviour
{
    public static InventorySlots Instance;
    public InputActionAsset inputActions;
    private InputAction FastSendAction;
    private InputAction ThrowOutAction;
    private InputAction[] QuickSlots = new InputAction[6];
    public Image[] ImageSlots;
    public Image[] QuickAccessImageSlots;
    public int TotalSlots = 5;
    public int TotalClosetSlots = 5;
    public int[] IndexSlots;
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
        TotalSlots = 6;
        IndexSlots = new int[49];

        ModuleThrowOut = new ModuleThrowOut();

        for(int i = 0; i < 48; i++)
        {
            if(i <= TotalSlots - 1)
                Slots[i].SetActive(true);
            else
                Slots[i].SetActive(false);
        }
        

        string data = PlayerPrefs.GetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split = data.Split('|');

        for (int i = 0; i < TotalSlots; i++)
        {
            if (i < split.Length && split[i] != "")
                IndexSlots[i] = int.Parse(split[i]);
            else
                IndexSlots[i] = 0;
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

        for(int i = 0; i < ImageSlots.Length; i++)
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
            

            for (int i = 24; i < 48; i++)
            {
                if(i <= (TotalClosetSlots + 23))
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
                IndexSlots[24 + i] = Closet.Slots[i];
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

            return;
        }

        if(Closet != null && FastSendAction.IsPressed())
        {
            if(Index <= TotalSlots)
            {
                for(int i = 24; i < (TotalClosetSlots + 24); i++)
                {
                    if(IndexSlots[i] == 0)
                    {
                        IndexSlots[i] = IndexSlots[Index];
                        IndexSlots[Index] = 0;
                        Closet.Slots[i - 24] = IndexSlots[i];
                        Closet.UpdateCloset();

                        return;
                    }
                }
            }
            else if(Index >= 24)
            {
                for(int i = 0; i < TotalSlots; i++)
                {
                    if(IndexSlots[i] == 0)
                    {
                        IndexSlots[i] = IndexSlots[Index];
                        IndexSlots[Index] = 0;
                        Closet.Slots[Index - 24] = 0;
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
            int tempID = IndexSlots[PreviousSlot];
            IndexSlots[PreviousSlot] = IndexSlots[Index];
            IndexSlots[Index] = tempID;

            SlotsAllocations[CurrentSlot].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);
            SlotsAllocations[Index].color = new Color(55f / 255f, 55f / 255f, 55f / 255f);

            CurrentSlot = -1;
            PreviousSlot = -1;

            Description.text = "";
            Name.text = "";

            if(Closet != null)
            {
                for (int i = 0; i < Closet.TotalSlots; i++)
                {
                    Closet.Slots[i] = IndexSlots[24 + i];
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
    }

    public void OnDeath()
    {
        for(int d = 0; d < TotalSlots; d++)
        {
            if(IndexSlots[d] != 0 && IndexSlots[d] < 50)
            {
                Vector3 pos = new Vector3(
                SpawnPos.position.x + Random.Range(-0.4f, 0.4f),
                SpawnPos.position.y - 0.1f,
                SpawnPos.position.z + Random.Range(-0.4f, 0.4f));

                int ObjectID = -1;

                for(int i = 4500; i < 12000; i++)
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
                ImageSlots[d].color = new Color(0, 0, 0, 0f);

                if(Closet != null && CurrentSlot != -1)
                    Closet.Slots[CurrentSlot] = 0;

                Description.text = "";
                Name.text = "";
            }
        }
    }
}
