using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public AllID AllID;
    public CenterSpawnedObjects CenterSpawnedObjects;
    public Closet[] ClosetPrefabs;
    public InventorySlots InventorySlots;

    void Start()
    {
        Invoke("Spawn", 0.1f);
    }

    public void Spawn()
    {
        for(int i = 4500; i < 12000; i++)
        {
            if(CenterSpawnedObjects.ResourcesID[i] == 1)
            {
                Objects obj = Instantiate(AllID.Prefab[CenterSpawnedObjects.ResourcesTypes[i]], CenterSpawnedObjects.ResourcesPositions[i], Quaternion.identity);

                obj.ObjectID = i;
                obj.Spawned = true;
                obj.ID = CenterSpawnedObjects.ResourcesTypes[i];
                obj.AllID = AllID;
                obj.CenterSpawnedObjects = CenterSpawnedObjects;
            }
        }

        for(int i = 13000; i < CenterSpawnedObjects.ResourcesID.Length; i++)
        {
            if(CenterSpawnedObjects.ResourcesID[i] == 1)
            {
                Closet closet = Instantiate(ClosetPrefabs[CenterSpawnedObjects.ResourcesTypes[i]], CenterSpawnedObjects.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.ResourcesRotations[i])).GetComponent<Closet>();

                closet.Spawned = true;
                closet.ID = i - 12000;
                closet.ClosetType = CenterSpawnedObjects.ResourcesTypes[i];
                closet.CenterSpawnedObjects = CenterSpawnedObjects;
                closet.InventorySlots = InventorySlots;
                closet.TotalSlots = CenterSpawnedObjects.ResourcesItems[i - 12000].Length;
                closet.Slots = new int[closet.TotalSlots];

                for(int j = 0; j < closet.TotalSlots; j++)
                {
                    closet.Slots[j] = CenterSpawnedObjects.ResourcesItems[i - 12000][j];
                }
            }
        }
    }
}
