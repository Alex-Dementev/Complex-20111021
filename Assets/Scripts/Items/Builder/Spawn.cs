using UnityEngine;

public class Spawn : MonoBehaviour
{
    public AllID AllID;
    public ChoiceOfBuilding ChoiceOfBuilding;

    [HideInInspector] public int Type;
    [HideInInspector] public Transform TransformObject;
    private int ObjectID;

    private int[] resource1 = new int[2];
    private int[] resource2 = new int[2];
    private int[] resource3 = new int[2];
    private int[] resource4 = new int[2];
    private bool ResourcesZero;



    private void ResourceValid(int[] Resource)
    {
        if(ResourcesZero || Resource[0] == 0)
            return;

        int count = 0;

        for(int i = 0; i < InventorySlots.Instance.TotalSlots; i++)
        {
            if(InventorySlots.Instance.IndexSlots[i] == Resource[0])
            {
                InventorySlots.Instance.IndexSlots[i] = 0;

                count++;

                if(count == Resource[1])
                    return;
            }
        }

        ResourcesZero = true;
    }

    public void SpawnObject()
    {
        if(Time.timeScale == 0)
            return;


        ResourcesZero = false;

        
        string[] Resources1 = AllID.BuildResource1[Type].Split('|');
        resource1[0] = int.Parse(Resources1[0]);
        if(Resources1[1] != "")
            resource1[1] = int.Parse(Resources1[1]);
        ResourceValid(resource1);

        string[] Resources2 = AllID.BuildResource2[Type].Split('|');
        resource2[0] = int.Parse(Resources2[0]);
        if(Resources2[1] != "")
            resource2[1] = int.Parse(Resources2[1]);
        ResourceValid(resource2);

        string[] Resources3 = AllID.BuildResource3[Type].Split('|');
        resource3[0] = int.Parse(Resources3[0]);
        if(Resources3[1] != "")
            resource3[1] = int.Parse(Resources3[1]);
        ResourceValid(resource3);

        string[] Resources4 = AllID.BuildResource4[Type].Split('|');
        resource4[0] = int.Parse(Resources4[0]);
        if(Resources4[1] != "")
            resource4[1] = int.Parse(Resources4[1]);
        ResourceValid(resource4);


        if(ResourcesZero)
        {
            Debug.Log("Ресурсы пропали!");
            return;
        }

        Debug.Log("Спавню!");

        ChoiceOfBuilding.Open();
        
        
        var obj = Instantiate(AllID.BuildPrefab[Type], TransformObject.position, TransformObject.rotation);


        for (int i = CenterSpawnedObjects.IDNotSpawnedBuilds; i < CenterSpawnedObjects.IDSpawnedBuilds; i++)
        {
            if (CenterSpawnedObjects.Instance.ResourcesID[i] == 0)
            {
                ObjectID = i - CenterSpawnedObjects.IDSpawnedObjects;
                CenterSpawnedObjects.Instance.ResourcesID[i] = 1;
                CenterSpawnedObjects.Instance.ResourcesTypes[i] = Type;
                break;
            }
        }

        switch (Type)
        {
            case 0:
            {
                var closet = obj.GetComponent<Closet>();
                CenterSpawnedObjects.Instance.ResourcesItems[ObjectID] = new int[24];

                closet.Spawned = true;
                closet.ClosetType = 0;
                closet.ID = ObjectID;

                closet.TotalSlots = 24;
                closet.Slots = new int[24];
                return;
            }

            case 1:
            {
                var closet = obj.GetComponent<Closet>();
                CenterSpawnedObjects.Instance.ResourcesItems[ObjectID] = new int[12];

                closet.Spawned = true;
                closet.ClosetType = 1;
                closet.ID = ObjectID;

                closet.TotalSlots = 12;
                closet.Slots = new int[12];
                return;
            }
        }
    }
}
