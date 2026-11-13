using UnityEngine;
using UnityEngine.UI;

public class Crafts : MonoBehaviour
{
    [HideInInspector] public bool IsActive;
    public static Crafts Instance;
    private int CurrentTable;
    private bool IsPush;
    private float Push;
    private int CurrentSlot;
    private AllID AllID;

    private int objectID;
    private float Speed = 1;
    private int[] Resources = new int[4];
    public float IsDelay;


    [Header("Крафты")]
    // тип|время|типРесурса1|колличество|2|коллич|3|коллич|4|коллич
    public string[] Table0;
    public string[] Table1;
    public string[] Table2;

    private string[] CurrentTables;

    [Header("Слоты")]
    public Image[] SlotsClicable;
    public Image[] SlotsSprites;
    public GameObject[] Slots;

    [Header("Остальное")]
    public GameObject Panel;
    public Animator PanelAnimator;
    public Slider PushSlider;
    public Text Name;
    public Text Description;
    public Text[] ResourcesText;
    public Text[] ResourcesCountText;
    public Image[] ResourcesImages;
    public Image ImageButton;

    
    

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        AllID = InventorySlots.Instance.AllID;
    }

    void Update()
    {
        IsDelay -= Time.unscaledDeltaTime;

        if(IsPush)
        {
            Push += Time.unscaledDeltaTime * Speed;

            if(Push >= 1)
            {
                string[] Craft = CurrentTables[CurrentSlot].Split('|');
            
                if(int.Parse(Craft[2]) != 0 && CountItems(int.Parse(Craft[2])) < int.Parse(Craft[3]))
                    Push = 0; 
                if(int.Parse(Craft[4]) != 0 && CountItems(int.Parse(Craft[4])) < int.Parse(Craft[5]))
                    Push = 0; 
                if(int.Parse(Craft[6]) != 0 && CountItems(int.Parse(Craft[6])) < int.Parse(Craft[7]))
                    Push = 0; 
                if(int.Parse(Craft[8]) != 0 && CountItems(int.Parse(Craft[8])) < int.Parse(Craft[9]))
                    Push = 0;

                if(Push >= 1)
                {
                    CountItemsDestroy(int.Parse(Craft[2]), int.Parse(Craft[3]));
                    CountItemsDestroy(int.Parse(Craft[4]), int.Parse(Craft[5]));
                    CountItemsDestroy(int.Parse(Craft[6]), int.Parse(Craft[7]));
                    CountItemsDestroy(int.Parse(Craft[8]), int.Parse(Craft[9]));
                    for(int i = 0; i < 28; i++)
                    {
                        if(InventorySlots.Instance.IndexSlots[i] == 0)
                        {
                            InventorySlots.Instance.IndexSlots[i] = int.Parse(Craft[0]);
                            InventorySlots.Instance.UpplyQuickAccess(i);
                            InventorySlots.Instance.UpplySlots(i);
                            break;
                        }
                    }
                    ClickToSlot(CurrentSlot);
                    IsPush = false;
                    Push = 0;
                } 
            }
        }
        else
            Push = 0;

        PushSlider.value = Push;
    }

    public void Activate(int action = 0)
    {
        if(IsDelay >= 0)
            return;

        if(IsActive)
        {
            IsActive = false;
            PanelAnimator.CrossFade("Close", 0.2f);
            PauseController.Speed = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SlotsClicable[CurrentSlot].color = new Color(55f/255f, 55f/255f, 55f/255f);
        }
        else if((DestroyPreviewModels.Destroy?.GetInvocationList().Length ?? 0) <= 1)
        {
            IsActive = true;
            Panel.SetActive(true);
            PanelAnimator.CrossFade("Open", 0.2f);
            PauseController.Speed = 0;
            CurrentTable = action;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Name.text = "";
            Description.text = "";
            ResourcesText[0].text = "";
            ResourcesText[1].text = "";
            ResourcesText[2].text = "";
            ResourcesText[3].text = "";
            ResourcesImages[0].sprite = AllID.Sprites[0];
            ResourcesImages[1].sprite = AllID.Sprites[0];
            ResourcesImages[2].sprite = AllID.Sprites[0];
            ResourcesImages[3].sprite = AllID.Sprites[0];
            ResourcesCountText[0].text = "";
            ResourcesCountText[1].text = "";
            ResourcesCountText[2].text = "";
            ResourcesCountText[3].text = "";
            Resources[0] = 0;
            Resources[1] = 0;
            Resources[2] = 0;
            Resources[3] = 0;
            CurrentSlot = 0;

            if(CurrentTable == 0)
                CurrentTables = Table0;
            else if(CurrentTable == 1)
                CurrentTables = Table1;
            else if(CurrentTable == 2)
                CurrentTables = Table2;

            
            for(int i = 0; i < SlotsClicable.Length; i++)
            {
                if(CurrentTables.Length <= i)
                    Slots[i].SetActive(false);
                else
                    ReloadModuls(i);
            }
        }

        IsDelay = 0.5f;
    }

    public void ReloadModuls(int slot)
    {
        string[] Craft = CurrentTables[slot].Split('|');

        Slots[slot].SetActive(true);

        if(Craft[0] != null || Craft[0] != "")
            SlotsSprites[slot].sprite = AllID.Sprites[int.Parse(Craft[0])];
    }

    public void Pushed(bool IsPushed)
    {
        if(!IsPush)
        {
            string[] Craft = CurrentTables[CurrentSlot].Split('|');
            
            if(int.Parse(Craft[2]) != 0 && CountItems(int.Parse(Craft[2])) < int.Parse(Craft[3]))
                return;
            if(int.Parse(Craft[4]) != 0 && CountItems(int.Parse(Craft[4])) < int.Parse(Craft[5]))
                return;
            if(int.Parse(Craft[6]) != 0 && CountItems(int.Parse(Craft[6])) < int.Parse(Craft[7]))
                return;
            if(int.Parse(Craft[8]) != 0 && CountItems(int.Parse(Craft[8])) < int.Parse(Craft[9]))
                return;
        }

        IsPush = IsPushed;
    }

    public void ClickToSlot(int slot = 0)
    {
        SlotsClicable[CurrentSlot].color = new Color(55f/255f, 55f/255f, 55f/255f);
        CurrentSlot = slot;
        SlotsClicable[CurrentSlot].color = new Color(85f/255f, 85f/255f, 85f/255f);

        string[] Craft = CurrentTables[CurrentSlot].Split('|');

        Speed = float.Parse(Craft[1]);

        Name.text = AllID.Names[int.Parse(Craft[0])];
        Description.text = AllID.Descriptions[int.Parse(Craft[0])];

        if(int.Parse(Craft[2]) != 0)
        {
            ResourcesText[0].text = AllID.Names[int.Parse(Craft[2])];
            ResourcesImages[0].sprite = AllID.Sprites[int.Parse(Craft[2])];
            Resources[0] = CountItems(int.Parse(Craft[2]));
            ResourcesCountText[0].text = Resources[0] + "/" + int.Parse(Craft[3]);
        }
        else
        {
            ResourcesText[0].text = "";
            ResourcesImages[0].sprite = AllID.Sprites[0];
            Resources[0] = 0;
            ResourcesCountText[0].text = "";
        }
        if(int.Parse(Craft[4]) != 0)
        {
            ResourcesText[1].text = AllID.Names[int.Parse(Craft[4])];
            ResourcesImages[1].sprite = AllID.Sprites[int.Parse(Craft[4])];
            Resources[1] = CountItems(int.Parse(Craft[4]));
            ResourcesCountText[1].text = Resources[1] + "/" + int.Parse(Craft[5]);
        }
        else
        {
            ResourcesText[1].text = "";
            ResourcesImages[1].sprite = AllID.Sprites[0];
            Resources[1] = 1;
            ResourcesCountText[1].text = "";
        }
        if(int.Parse(Craft[6]) != 0)
        {
            ResourcesText[2].text = AllID.Names[int.Parse(Craft[6])];
            ResourcesImages[2].sprite = AllID.Sprites[int.Parse(Craft[6])];
            Resources[2] = CountItems(int.Parse(Craft[6]));
            ResourcesCountText[2].text = Resources[2] + "/" + int.Parse(Craft[7]);
        }
        else
        {
            ResourcesText[2].text = "";
            ResourcesImages[2].sprite = AllID.Sprites[0];
            Resources[2] = 2;
            ResourcesCountText[2].text = "";
        }
        if(int.Parse(Craft[8]) != 0)
        {
            ResourcesText[3].text = AllID.Names[int.Parse(Craft[8])];
            ResourcesImages[3].sprite = AllID.Sprites[int.Parse(Craft[8])];
            Resources[3] = CountItems(int.Parse(Craft[8]));
            ResourcesCountText[3].text = Resources[3] + "/" + int.Parse(Craft[9]);
        }
        else
        {
            ResourcesText[3].text = "";
            ResourcesImages[3].sprite = AllID.Sprites[0];
            Resources[3] = 3;
            ResourcesCountText[3].text = "";
        }
    }

    public int CountItems(int ID)
    {
        InventorySlots IS = InventorySlots.Instance;
        int Count = 0;

        for(int i = 0; i < 28; i++)
        {
            if(IS.IndexSlots[i] == ID)
                Count++;
        }

        return Count;
    }
    public void CountItemsDestroy(int ID, int count)
    {
        InventorySlots IS = InventorySlots.Instance;
        int Count = 0;

        for(int i = 0; i < 28; i++)
        {
            if(IS.IndexSlots[i] == ID)
            {
                IS.IndexSlots[i] = 0;
                IS.UpplyQuickAccess(i);
                IS.UpplySlots(i);
                Count++;
            }

            if(Count >= count)
                return;
        }
    }
}
