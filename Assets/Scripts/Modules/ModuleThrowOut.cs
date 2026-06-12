using UnityEngine;

public class ModuleThrowOut
{
    private Vector3 pos;
    public void ThrowOut(bool InInventory = true)
    {
        if(InInventory)
        {
            var inv = InventorySlots.Instance;

            if(inv.CurrentSlot == -1)
                return;

            int id = inv.IndexSlots[inv.CurrentSlot];

            if(id == 0)
                return;

            pos = new Vector3(inv.SpawnPos.position.x + Random.Range(-0.4f, 0.4f), inv.SpawnPos.position.y, inv.SpawnPos.position.z + Random.Range(-0.4f, 0.4f));

            int objectID = -1;

            for(int i = CenterSpawnedObjects.IDNotSpawnedObjects; i < CenterSpawnedObjects.IDSpawnedObjects; i++)
            {
                if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
                {
                    objectID = i;
                    break;
                }
            }

            Objects obj = Object.Instantiate(inv.AllID.Prefab[id], pos, Quaternion.identity);

            CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
            CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = pos;
            CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = id;
            CenterSpawnedObjects.Instance.ResourcesSettings[objectID] = inv.SettingsSlots[inv.CurrentSlot];

            obj.ObjectID = objectID;
            obj.Spawned = true;
            obj.ID = id;
            obj.AllID = inv.AllID;
            obj.Settings = inv.SettingsSlots[inv.CurrentSlot];

            inv.IndexSlots[inv.CurrentSlot] = 0;
            inv.SettingsSlots[inv.CurrentSlot] = 0;

            if(inv.Closet != null)
                inv.Closet.Slots[inv.CurrentSlot - 28] = 0;

            inv.ClickToSlot(inv.CurrentSlot);
        }
        else
        {
            var inv = InventorySlots.Instance;

            pos = new Vector3(inv.SpawnPos.position.x + Random.Range(-0.1f, 0.1f), inv.SpawnPos.position.y + 0.4f, inv.SpawnPos.position.z + 0.3f);

            int objectID = -1;

            for(int i = CenterSpawnedObjects.IDNotSpawnedObjects; i < CenterSpawnedObjects.IDSpawnedObjects; i++)
            {
                if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
                {
                    objectID = i;
                    break;
                }
            }

            if(objectID == -1)
                return;

            Objects obj = Object.Instantiate(inv.AllID.Prefab[inv.SpawnedID], pos, Quaternion.identity);

            CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
            CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = pos;
            CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = inv.SpawnedID;
            CenterSpawnedObjects.Instance.ResourcesSettings[objectID] = inv.SettingsSlots[0];

            obj.ObjectID = objectID;
            obj.Spawned = true;
            obj.ID = inv.SpawnedID;
            obj.AllID = inv.AllID;
            obj.Settings = inv.SettingsSlots[0];

            inv.IndexSlots[0] = 0;
            inv.SettingsSlots[0] = 0;
            

            Rigidbody rb = obj.GetComponent<Rigidbody>();

            rb.AddForce((inv.SpawnPos.forward + Vector3.up * 0.25f) * 5f, ForceMode.Impulse);
        }
    }

    public void SpawnResourcetAfterDestroy()
    {
        var inv = InventorySlots.Instance;

        for(int i = 0; i < inv.TotalSlots; i++)
        {
            if(inv.IndexSlots[i] == 0)
            {
                inv.IndexSlots[i] = inv.SpawnedID;
                inv.SettingsSlots[i] = inv.AllID.Settings[inv.SpawnedID];
                return;
            }
        }

        pos = new Vector3(inv.SpawnPos.position.x + Random.Range(-0.4f, 0.4f), inv.SpawnPos.position.y, inv.SpawnPos.position.z + Random.Range(-0.4f, 0.4f));

        int objectID = -1;

        for(int i = CenterSpawnedObjects.IDNotSpawnedObjects; i < CenterSpawnedObjects.IDSpawnedObjects; i++)
        {
            if(CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
            {
                objectID = i;
                break;
            }
        }

        if(objectID == -1)
            return;

        Objects obj = Object.Instantiate(inv.AllID.Prefab[inv.SpawnedID], pos, Quaternion.identity);

        CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
        CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = pos;
        CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = inv.SpawnedID;
        CenterSpawnedObjects.Instance.ResourcesSettings[objectID] = inv.AllID.Settings[inv.SpawnedID];

        obj.ObjectID = objectID;
        obj.Spawned = true;
        obj.ID = inv.SpawnedID;
        obj.AllID = inv.AllID;
        obj.Settings = inv.AllID.Settings[inv.SpawnedID];
    }
}