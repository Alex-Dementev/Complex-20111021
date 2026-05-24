using UnityEngine;

public class ModuleThrowOut
{
    public void ThrowOut()
    {
        var inv = InventorySlots.Instance;

        if(inv.CurrentSlot == -1)
            return;

        int id = inv.IndexSlots[inv.CurrentSlot];

        if(id == 0)
            return;

        Vector3 pos = new Vector3(
            inv.Player.position.x + Random.Range(-0.5f, 0.5f),
            inv.Player.position.y + 0.2f,
            inv.Player.position.z + Random.Range(-0.5f, 0.5f)
        );

        int objectID = -1;

        for(int i = 4500; i < 12000; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
            {
                objectID = i;
                break;
            }
        }

        Objects obj = Object.Instantiate(
            inv.AllID.Prefab[id],
            pos,
            Quaternion.identity
        );

        CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
        CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = pos;
        CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = id;

        obj.ObjectID = objectID;
        obj.Spawned = true;
        obj.ID = id;
        obj.AllID = inv.AllID;

        inv.IndexSlots[inv.CurrentSlot] = 0;
        inv.ImageSlots[inv.CurrentSlot].color = new Color(0,0,0,0f);

        if(inv.Closet != null)
            inv.Closet.Slots[inv.CurrentSlot - 24] = 0;

        inv.ClickToSlot(inv.CurrentSlot);
    }

    public void SpawnResourcetAfterDestroy()
    {
        var inv = InventorySlots.Instance;

        for(int i = 0; i < inv.TotalSlots; i++)
        {
            if(inv.IndexSlots[i] == 0)
            {
                inv.IndexSlots[i] = inv.SpawnedID;
                return;
            }
        }

        Vector3 pos = new Vector3(
            inv.Player.position.x + Random.Range(-0.5f, 0.5f),
            inv.Player.position.y + 0.2f,
            inv.Player.position.z + Random.Range(-0.5f, 0.5f)
        );

        int objectID = -1;

        for(int i = 4500; i < 12000; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
            {
                objectID = i;
                break;
            }
        }

        if(objectID == -1)
            return;

        Objects obj = Object.Instantiate(
            inv.AllID.Prefab[inv.SpawnedID],
            pos,
            Quaternion.identity
        );

        CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
        CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = pos;
        CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = inv.SpawnedID;

        obj.ObjectID = objectID;
        obj.Spawned = true;
        obj.ID = inv.SpawnedID;
        obj.AllID = inv.AllID;
    }
}