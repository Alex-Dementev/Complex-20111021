using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image[] ImageSlots;
    [HideInInspector]public int TotalSlots = 5;
    [HideInInspector]public int TotalClosetSlots = 5;
    [HideInInspector]public int[] IndexSlots;
    public GameObject[] Slots;
    public Image[] SlotsAllocations;
    public Text Name;
    public Text Description;
    public AllID AllID;
    private int CurrentSlot = -1;
    private int PreviousSlot = -1;
    [HideInInspector] public int[] ClosetSlots;
    [HideInInspector] public Closet Closet;

    [Header("Ссылки для спавна")]
    public Transform Player;
    public CenterSpawnedObjects CenterSpawnedObjects;
    public int SpawnedID;


    private void Start()
    {
        TotalSlots = 6;
        IndexSlots = new int[49];

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
    }

    void Update()
    {
        for(int i = 0; i < ImageSlots.Length; i++)
        {
            if(ImageSlots[i] != null)
                ImageSlots[i].color = AllID.Colors[IndexSlots[i]];
        }
    }

    public void UpdateCloset()
    {
        if(Closet != null)
        {
            TotalClosetSlots = Closet.TotalSlots;
            TotalClosetSlots += 24;
            

            for (int i = 24; i < 48; i++)
            {
                if(i <= TotalClosetSlots - 1)
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

            if(Index != CurrentSlot)
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

    public void ThrowOut()
    {
        if(CurrentSlot != -1)
        {
            if(IndexSlots[CurrentSlot] != 0)
            {
                Vector3 pos = new Vector3(
                Player.position.x + Random.Range(-0.5f, 0.5f),
                Player.position.y + 0.2f,
                Player.position.z + Random.Range(-0.5f, 0.5f));

                int ObjectID = -1;

                for(int i = 4500; i < 12000; i++)
                {
                    if(CenterSpawnedObjects.ResourcesID[i] == 0)
                    {
                        ObjectID = i;
                        break;
                    }
                }

                Objects obj = Instantiate(AllID.Prefab[IndexSlots[CurrentSlot]], pos, Quaternion.identity);

                CenterSpawnedObjects.ResourcesID[ObjectID] = 1;
                CenterSpawnedObjects.ResourcesPositions[ObjectID] = pos;
                CenterSpawnedObjects.ResourcesTypes[ObjectID] = IndexSlots[CurrentSlot];

                obj.ObjectID = ObjectID;
                obj.Spawned = true;
                obj.ID = IndexSlots[CurrentSlot];
                obj.AllID = AllID;
                obj.CenterSpawnedObjects = CenterSpawnedObjects;

                IndexSlots[CurrentSlot] = 0;
                ImageSlots[CurrentSlot].color = new Color(0, 0, 0, 0f);

                if(Closet != null)
                    Closet.Slots[CurrentSlot - 24] = 0;

                ClickToSlot(CurrentSlot);
            }
        }
    }

    public void SpawnResourcetAfterDestroy()
    {
        for(int i = 0; i < TotalSlots; i++)
        {
            if(IndexSlots[i] == 0)
            {
                IndexSlots[i] = SpawnedID;
                return;
            }
        }

        Vector3 pos = new Vector3(
        Player.position.x + Random.Range(-0.5f, 0.5f),
        Player.position.y + 0.2f,
        Player.position.z + Random.Range(-0.5f, 0.5f));

        int ObjectID = -1;

        for(int i = 4500; i < 12000; i++)
        {
            if(CenterSpawnedObjects.ResourcesID[i] == 0)
            {
                ObjectID = i;
                break;
            }
        }

        Objects obj = Instantiate(AllID.Prefab[SpawnedID], pos, Quaternion.identity);

        CenterSpawnedObjects.ResourcesID[ObjectID] = 1;
        CenterSpawnedObjects.ResourcesPositions[ObjectID] = pos;
        CenterSpawnedObjects.ResourcesTypes[ObjectID] = SpawnedID;

        obj.ObjectID = ObjectID;
        obj.Spawned = true;
        obj.ID = SpawnedID;
        obj.AllID = AllID;
        obj.CenterSpawnedObjects = CenterSpawnedObjects;
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
                Player.position.x + Random.Range(-0.5f, 0.5f),
                Player.position.y + 0.2f,
                Player.position.z + Random.Range(-0.5f, 0.5f));

                int ObjectID = -1;

                for(int i = 4500; i < 12000; i++)
                {
                    if(CenterSpawnedObjects.ResourcesID[i] == 0)
                    {
                        ObjectID = i;
                        break;
                    }
                }

                Objects obj = Instantiate(AllID.Prefab[IndexSlots[d]], pos, Quaternion.identity);

                CenterSpawnedObjects.ResourcesID[ObjectID] = 1;
                CenterSpawnedObjects.ResourcesPositions[ObjectID] = pos;
                CenterSpawnedObjects.ResourcesTypes[ObjectID] = IndexSlots[d];

                obj.ObjectID = ObjectID;
                obj.Spawned = true;
                obj.ID = IndexSlots[d];
                obj.AllID = AllID;
                obj.CenterSpawnedObjects = CenterSpawnedObjects;

                IndexSlots[d] = 0;
                ImageSlots[d].color = new Color(0, 0, 0, 0f);

                if(Closet != null)
                    Closet.Slots[CurrentSlot] = 0;

                Description.text = "";
                Name.text = "";
            }
        }
    }
}
