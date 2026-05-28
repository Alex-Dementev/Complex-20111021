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
        
        for(int i = 4500; i < 12000; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 1)
            {
                Objects obj = Instantiate(AllID.Prefab[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.identity);

                obj.ObjectID = i;
                obj.Spawned = true;
                obj.ID = CenterSpawnedObjects.Instance.ResourcesTypes[i];
                obj.AllID = AllID;
            }
        }

        for(int i = 13000; i < CenterSpawnedObjects.Instance.ResourcesID.Length; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 1)
            {
                Closet closet = Instantiate(ClosetPrefabs[CenterSpawnedObjects.Instance.ResourcesTypes[i]], CenterSpawnedObjects.Instance.ResourcesPositions[i], Quaternion.Euler(CenterSpawnedObjects.Instance.ResourcesRotations[i])).GetComponent<Closet>();

                closet.Spawned = true;
                closet.ID = i - 12000;
                closet.ClosetType = CenterSpawnedObjects.Instance.ResourcesTypes[i];
                closet.TotalSlots = CenterSpawnedObjects.Instance.ResourcesItems[i - 12000].Length;
                closet.Slots = new int[closet.TotalSlots];

                for(int j = 0; j < closet.TotalSlots; j++)
                {
                    closet.Slots[j] = CenterSpawnedObjects.Instance.ResourcesItems[i - 12000][j];
                }
            }
        }
    }
}
