using UnityEngine;

public class StartSpawn : MonoBehaviour
{
    public AllID AllID;
    public Closet[] ClosetPrefabs;
    public InventorySlots InventorySlots;
    private bool Load;

    void Update()
    {
        if(!Load && CenterSpawnedObjects.Load)
            Spawn();
    }

    public void Spawn()
    {
        Load = true;
        
        for(int i = CenterSpawnedObjects.IDNotSpawnedObjects; i < CenterSpawnedObjects.IDSpawnedObjects; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 1)
            {
                Objects obj = Instantiate(AllID.Prefab[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.identity);

                obj.ObjectID = i;
                obj.Spawned = true;
                obj.ID = CenterSpawnedObjects.Instance.ResourcesTypes[i];
            }
        }

        for(int i = CenterSpawnedObjects.IDNotSpawnedBuilds; i < CenterSpawnedObjects.IDSpawnedBuilds; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 1)
            {
                switch (CenterSpawnedObjects.Instance.ResourcesTypes[i])
                {
                    case 0:
                    {
                        Closet closet = Instantiate(ClosetPrefabs[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<Closet>();

                        closet.Spawned = true;
                        closet.ID = i - CenterSpawnedObjects.IDSpawnedObjects;
                        closet.ClosetType = CenterSpawnedObjects.Instance.ResourcesTypes[i];
                        closet.TotalSlots = CenterSpawnedObjects.Instance.ResourcesItems[i - CenterSpawnedObjects.IDSpawnedObjects].Length;
                        closet.Slots = new int[closet.TotalSlots];

                        for(int j = 0; j < closet.TotalSlots; j++)
                        {
                            closet.Slots[j] = CenterSpawnedObjects.Instance.ResourcesItems[i - CenterSpawnedObjects.IDSpawnedObjects][j];
                        }
                        break;
                    }
                    case 1:
                    {
                        Closet closet = Instantiate(ClosetPrefabs[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<Closet>();

                        closet.Spawned = true;
                        closet.ID = i - CenterSpawnedObjects.IDSpawnedObjects;
                        closet.ClosetType = CenterSpawnedObjects.Instance.ResourcesTypes[i];
                        closet.TotalSlots = CenterSpawnedObjects.Instance.ResourcesItems[i - CenterSpawnedObjects.IDSpawnedObjects].Length;
                        closet.Slots = new int[closet.TotalSlots];

                        for(int j = 0; j < closet.TotalSlots; j++)
                        {
                            closet.Slots[j] = CenterSpawnedObjects.Instance.ResourcesItems[i - CenterSpawnedObjects.IDSpawnedObjects][j];
                        }
                        break;
                    }
                    case 2:
                    {
                        RefillOxygen RefillOxygen = Instantiate(AllID.BuildPrefab[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<RefillOxygen>();

                        RefillOxygen.Spawned = true;
                        RefillOxygen.ID = i - CenterSpawnedObjects.IDSpawnedObjects;
                        break;
                    }
                    case 3:
                    {
                        BackGroundObjects BackGroundObjects = Instantiate(AllID.BuildPrefab[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<BackGroundObjects>();

                        BackGroundObjects.Spawned = true;
                        BackGroundObjects.ID = i - CenterSpawnedObjects.IDSpawnedObjects;
                        break;
                    }
                    case 4:
                    {
                        ChargeButteriesBase ChargeButteriesBase = Instantiate(AllID.BuildPrefab[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<ChargeButteriesBase>();

                        ChargeButteriesBase.Spawned = true;
                        ChargeButteriesBase.ID = i - CenterSpawnedObjects.IDSpawnedObjects;
                        break;
                    }
                }
            }
        }
    }
}
