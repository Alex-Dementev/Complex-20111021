using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ChoiceOfBuilding : MonoBehaviour
{
    public static ChoiceOfBuilding Instance;

    public GameObject[] Panels;
    public Image[] Images;
    public Text[] Names;
    public Text[] ResourcesTexts;
    public Image[] ResourcesImages;
    public Text Descriptions;
    public int Current;

    public AllID AllID;

    public InputActionAsset inputActions;
    private InputAction ScrollMouseAction;
    private InputAction MiddleMouseAction;
    private InputAction EscapePreviewModeAction;
    private int Direction;
    private Vector2 scroll;

    public int CurrentBuildLevel;

    private int CurrentPanel;

    public GameObject Panel;
    public Animator PanelAnimator;
    public bool IsActive;

    private int[] resource1 = new int[2];
    private int[] resource2 = new int[2];
    private int[] resource3 = new int[2];
    private int[] resource4 = new int[2];
    private bool ResourcesZero;
    private int BuilderID = -1;


    void Start()
    {
        Instance = this;

        var playerMap = inputActions.FindActionMap("Player");
        ScrollMouseAction = playerMap.FindAction("ScrollMouse");
        MiddleMouseAction = playerMap.FindAction("СКМ");
        EscapePreviewModeAction = playerMap.FindAction("EscapePreviewMode");
        ScrollMouseAction.Enable();
        MiddleMouseAction.Enable();
        EscapePreviewModeAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0)
            return;


        if(EscapePreviewModeAction.triggered && InventorySlots.Instance.IndexSlots[0] == BuilderID)
        {
            DestroyPreviewModels.Destroy?.Invoke(false);

            Open();
        }


        if(!IsActive)
            return;

        if(!Panel.activeSelf)
            Open();

        scroll = ScrollMouseAction.ReadValue<Vector2>();

        if(scroll.y >= 1)
        {
            DirectionPlus1();
        }
        else if(scroll.y <= -1)
        {
            DirectionMinus1();
        }

        if(MiddleMouseAction.triggered)
        {
            ResourcesZero = false;


            string[] Resources1 = AllID.BuildResource1[Current].Split('|');
            resource1[0] = int.Parse(Resources1[0]);
            if(Resources1[1] != "")
                resource1[1] = int.Parse(Resources1[1]);
            ResourceValid(resource1);

            string[] Resources2 = AllID.BuildResource2[Current].Split('|');
            resource2[0] = int.Parse(Resources2[0]);
            if(Resources2[1] != "")
                resource2[1] = int.Parse(Resources2[1]);
            ResourceValid(resource2);

            string[] Resources3 = AllID.BuildResource3[Current].Split('|');
            resource3[0] = int.Parse(Resources3[0]);
            if(Resources3[1] != "")
                resource3[1] = int.Parse(Resources3[1]);
            ResourceValid(resource3);

            string[] Resources4 = AllID.BuildResource4[Current].Split('|');
            resource4[0] = int.Parse(Resources4[0]);
            if(Resources4[1] != "")
                resource4[1] = int.Parse(Resources4[1]);
            ResourceValid(resource4);


            if(ResourcesZero)
            {
                Debug.Log("Бомжара!");
                return;
            }

            Debug.Log("Ресурсы есть!");

            var obj = Instantiate(AllID.PreviewPrefab[Current], Vector3.zero, Quaternion.identity);
            BuildManager.PreviewMode = obj.GetComponent<PreviewMode>();
            BuildManager.previewObject = obj.transform;

            Close();
        }
    }

    private void ResourceValid(int[] Resource)
    {
        if(ResourcesZero || Resource[0] == 0)
            return;

        int count = 0;

        for(int i = 0; i < InventorySlots.Instance.TotalSlots; i++)
        {
            if(InventorySlots.Instance.IndexSlots[i] == Resource[0])
            {
                count++;

                if(count >= Resource[1])
                    return;
            }
        }

        ResourcesZero = true;
    }

    public void Open()
    {
        if(DestroyPreviewModels.Destroy.GetInvocationList().Length != 1)
            return;

        Panel.SetActive(true);
        PanelAnimator.CrossFade("Open", 0.1f);
        IsActive = true;

        CurrentPanel = 0;

        if(BuilderID == -1)
        {
            for(int i = 0; i < AllID.BuildName.Length; i++)
            {
                if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
                {
                    Current = i;
                    CurrentPanel = 2;

                    Descriptions.text = AllID.BuildDescription[i];

                    LoadResources(i);
                    
                    Panels[0].SetActive(false);

                    break;
                }
            }

            for(int i = Current + 1; i < AllID.BuildName.Length; i++)
            {
                if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
                {
                    CurrentPanel++;

                    LoadResources(i);
                }
            }

            while(CurrentPanel <= 4)
            {
                Panels[CurrentPanel].SetActive(false);
                CurrentPanel++;
            }
        }

        BuilderID = InventorySlots.Instance.IndexSlots[0];
    }
    public void Close()
    {
        PanelAnimator.CrossFade("Close", 0.2f);
        IsActive = false;
    }
    public void DirectionPlus1()
    {
        if(AllID.BuildName.Length <= Current + 1)
            return;

        CurrentPanel = 0;

        for(int i = Current + 1; i < AllID.BuildName.Length; i++)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                Current = i;
                CurrentPanel = 2;

                Descriptions.text = AllID.BuildDescription[i];

                LoadResources(i);

                Panels[0].SetActive(false);

                break;
            }
        }

        for(int i = Current + 1; i < AllID.BuildName.Length; i++)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                CurrentPanel++;

                LoadResources(i);
            }
        }

        while(CurrentPanel <= 4)
        {
            Panels[CurrentPanel].SetActive(false);
            CurrentPanel++;
        }

        CurrentPanel = 0;

        for(int i = Current - 1; i > -1; i--)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                CurrentPanel = 1;

                LoadResources(i);

                break;
            }
        }

        if(CurrentPanel == 0)
            Panels[0].SetActive(false);
    }
    public void DirectionMinus1()
    {
        if(Current == 0)
            return;

        CurrentPanel = 0;

        for(int i = Current - 1; i < AllID.BuildName.Length; i--)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                Current = i;
                CurrentPanel = 2;

                Descriptions.text = AllID.BuildDescription[i];

                LoadResources(i);

                Panels[0].SetActive(false);

                break;
            }
        }

        for(int i = Current + 1; i < AllID.BuildName.Length; i++)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                CurrentPanel++;

                LoadResources(i);
            }
        }

        while(CurrentPanel <= 4)
        {
            Panels[CurrentPanel].SetActive(false);
            CurrentPanel++;
        }

        CurrentPanel = 0;

        for(int i = Current - 1; i > -1; i--)
        {
            if(CurrentBuildLevel >= AllID.BuildLevel[i] && AllID.BuildScanned[i] == 1)
            {
                CurrentPanel = 1;

                LoadResources(i);

                break;
            }
        }

        if(CurrentPanel == 0)
            Panels[0].SetActive(false);
    }

    public void LoadResources(int i)
    {
        Panels[CurrentPanel - 1].SetActive(true);
        Names[CurrentPanel - 1].text = AllID.BuildName[i];
        Images[CurrentPanel - 1].sprite = AllID.BuildSprite[i];

        string[] Resources1 = AllID.BuildResource1[i].Split('|');
        ResourcesImages[4 * (CurrentPanel - 1) + 0].sprite = AllID.Sprites[int.Parse(Resources1[0])];
        ResourcesTexts[4 * (CurrentPanel - 1) + 0].text = Resources1[1];

        string[] Resources2 = AllID.BuildResource2[i].Split('|');
        ResourcesImages[4 * (CurrentPanel - 1) + 1].sprite = AllID.Sprites[int.Parse(Resources2[0])];
        ResourcesTexts[4 * (CurrentPanel - 1) + 1].text = Resources2[1];

        string[] Resources3 = AllID.BuildResource3[i].Split('|');
        ResourcesImages[4 * (CurrentPanel - 1) + 2].sprite = AllID.Sprites[int.Parse(Resources3[0])];
        ResourcesTexts[4 * (CurrentPanel - 1) + 2].text = Resources3[1];

        string[] Resources4 = AllID.BuildResource4[i].Split('|');
        ResourcesImages[4 * (CurrentPanel - 1) + 3].sprite = AllID.Sprites[int.Parse(Resources4[0])];
        ResourcesTexts[4 * (CurrentPanel - 1) + 3].text = Resources4[1];
    }
}
