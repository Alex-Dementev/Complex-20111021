using UnityEngine;

public class Closet : MonoBehaviour
{
    //public TMP_Text Name;
    public int[] Slots;
    public int TotalSlots;
    public bool Spawned;
    public int ClosetType;
    public CenterSpawnedObjects CenterSpawnedObjects;
    public int ID;
    private int Price;
    public InventorySlots InventorySlots;

    private float IsDelay;
    private float TimeIsDelay;
    private bool Delay;
    

    void Start()
    {
        Invoke("UpdateState", 0.2f);
    }

    public void UpdateState()
    {
        if(!Spawned)
        {
            Delay = true;

            if(CenterSpawnedObjects.ResourcesID[ID + 12000] == 1)
            {
                Destroy(gameObject);
            }

            if(ClosetType == 0)
            {
                TotalSlots = 24;
                Slots = new int[24];
                Price = 2;
            }
            else if(ClosetType == 1)
            {
                TotalSlots = 12;
                Slots = new int[12];
                Price = 1;
            }

            if (CenterSpawnedObjects.ResourcesItems[ID] == null)
                CenterSpawnedObjects.ResourcesItems[ID] = new int[Slots.Length];

            for(int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = CenterSpawnedObjects.ResourcesItems[ID][i];
            }

            CenterSpawnedObjects.ResourcesPositions[ID + 12000] = transform.position;
            CenterSpawnedObjects.ResourcesRotations[ID + 12000] = transform.eulerAngles;
        }
        else
        {
            if(ClosetType == 0)
            {
                TotalSlots = 24;
                Price = 2;
            }
            else if(ClosetType == 1)
            {
                TotalSlots = 12;
                Price = 1;
            }

            if (CenterSpawnedObjects.ResourcesItems[ID] == null)
                CenterSpawnedObjects.ResourcesItems[ID] = new int[Slots.Length]; 

            for(int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = CenterSpawnedObjects.ResourcesItems[ID][i];
            }
        }

        TimeIsDelay = Random.Range(1.5f, 2.5f);
        IsDelay = TimeIsDelay;
    }

    void Update()
    {
        IsDelay -= Time.deltaTime;

        if(IsDelay <= 0)
        {
            IsDelay = TimeIsDelay;

            if(Spawned)
                Delay = true;

            if(Delay)
                UpdateCloset();
        }
    }

    public void UpdateCloset()
    {
        CenterSpawnedObjects.ResourcesPositions[ID + 12000] = transform.position;
        CenterSpawnedObjects.ResourcesRotations[ID + 12000] = transform.eulerAngles;

        CenterSpawnedObjects.ResourcesTypes[ID + 12000] = ClosetType;

        if (CenterSpawnedObjects.ResourcesItems[ID] == null || CenterSpawnedObjects.ResourcesItems[ID].Length != Slots.Length)
        {
            CenterSpawnedObjects.ResourcesItems[ID] = new int[Slots.Length]; 
        }
        
        for(int i = 0; i < Slots.Length; i++)
        {
            CenterSpawnedObjects.ResourcesItems[ID][i] = Slots[i];
        }
    }

    public void DestroyCloset()
    {
        for(int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i] != 0)
                return;
        }

        if(Spawned)
            CenterSpawnedObjects.ResourcesID[ID + 12000] = 0;
        else
            CenterSpawnedObjects.ResourcesID[ID + 12000] = 1;

        CenterSpawnedObjects.ResourcesPositions[ID + 12000] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.ResourcesRotations[ID + 12000] = new Vector3(0, 0, 0);
        CenterSpawnedObjects.ResourcesTypes[ID + 12000] = 0;

        CenterSpawnedObjects.ResourcesNames[ID] = null;
        CenterSpawnedObjects.ResourcesItems[ID] = null;

        InventorySlots.SpawnedID = 2;

        for(int i = 0; i < Price; i++)
        {
            InventorySlots.SpawnResourcetAfterDestroy();
        }

        Destroy(gameObject);
    }
}
