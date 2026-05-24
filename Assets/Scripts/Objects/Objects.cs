using UnityEngine;

public class Objects : MonoBehaviour, IInteractable
{
    public int ID;
    public int ObjectID;
    public string Name;
    public AllID AllID;
    public bool Spawned;
    private bool Saved;
    private bool Load;


    void Start()
    {
        Name = AllID.Names[ID];
    }

    // Update is called once per frame
    void Update()
    {
        if(CenterSpawnedObjects.Load)
        {
            if(!Load)
                UpdateState();

            if(PauseController.OpenPause && !Saved)
            {
                CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID] = transform.position;
                CenterSpawnedObjects.Instance.ResourcesRotations[ObjectID] = transform.eulerAngles;
                Saved = true;
            }
            else if(!PauseController.OpenPause)
                Saved = false;
        }
    }

    public void LeftClick()
    {
        bool foundSlot = false;
        for(int i = 0; i < InventorySlots.Instance.TotalSlots; i++)
        {
            if(InventorySlots.Instance.IndexSlots[i] == 0)
            {
                InventorySlots.Instance.IndexSlots[i] = ID;
                foundSlot = true;
                break;
            }
        }

        if(!foundSlot)
        {
            LiftAnObject.Instance.StartTrableAnimator("Инвентарь полон");
            return;
        }


        if(!Spawned)
        {
            CenterSpawnedObjects.Instance.ResourcesID[ObjectID] = 1;
            CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.Instance.ResourcesRotations[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.Instance.ResourcesTypes[ObjectID] = 0;
        }
        else
        {
            CenterSpawnedObjects.Instance.ResourcesID[ObjectID] = 0;
            CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.Instance.ResourcesRotations[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.Instance.ResourcesTypes[ObjectID] = 0;
        }

        Destroy(gameObject);
    }

    public void UpdateState()
    {
        Load = true;

        if(CenterSpawnedObjects.Instance.ResourcesID[ObjectID] == 1 && !Spawned)
        {
            Destroy(gameObject);
            return;
        }

        if(CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID] != new Vector3(0, 0, 0))
        {
            transform.position = CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID];
            transform.rotation = Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesPositions[ObjectID]);
        }
    }

    public void RightClick()
    {
        
    }

    public string GetName()
    {
        return Name;
    }
}
