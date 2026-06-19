using UnityEngine;

public class BiomeResourcesSpawner : MonoBehaviour
{
    public int ID;

    public int[] Types;
    public int[] Settings;
    public int DistanceToSpawn = 100;

    private Collider Collider;
    private Bounds Bounds;
    private bool Load;
    private float randomX, randomZ;
    private RaycastHit hit;
    private Vector3 FinalPoint;
    private AllID AllID;
    private System.Random sysRandom;



    void Update()
    {
        if(!Load && CenterSpawnedObjects.Load)
        {
            Load = true;
            Invoke("SpawnRes", Random.Range(0.1f, 0.5f));
        }
    }

    private void SpawnRes()
    {
        if(CenterSpawnedObjects.Instance.BiomeSpawned[ID] == 1)
            return;


        Collider = GetComponent<Collider>();
        Bounds = Collider.bounds;

        AllID = InventorySlots.Instance.AllID;

        sysRandom = new System.Random();


        for(int i = 0; i < Types.Length; i++)
        {
            if(Types[i] != 0)
            {
                for(int j = 0; j < Types[i]; j++)
                {
                    Debug.Log("BiomeResourcesSpawner: Спавн ресурса ID " + Types[i] + " в биоме ID " + ID);
                    SpawnPos(i);
                }
            }
        }

        CenterSpawnedObjects.Instance.BiomeSpawned[ID] = 1;
    }

    private void SpawnPos(int i)
    {
        FinalPoint = Vector3.zero;

        for(int k = 0; k < 50; k++)
        {            
            randomX = (float)(sysRandom.NextDouble() * (Bounds.max.x - Bounds.min.x) + Bounds.min.x);
            randomZ = (float)(sysRandom.NextDouble() * (Bounds.max.z - Bounds.min.z) + Bounds.min.z);

            Vector3 spawnPoint = new Vector3(randomX, Bounds.max.y, randomZ);
            
            if (Physics.Raycast(spawnPoint, Vector3.down, out hit, DistanceToSpawn))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                Debug.DrawLine(spawnPoint, hit.point, Color.red, 5f);

                if(angle < 45f)
                {
                    FinalPoint = hit.point + Vector3.up * 0.5f;
                    Spawn(i);
                    return;
                }
            }
        }
    }

    private void Spawn(int i)
    {
        if(FinalPoint != Vector3.zero)
        {
            Objects obj = Instantiate(AllID.Prefab[i], FinalPoint, Quaternion.identity);

            int objectID = -1;      

            for(int l = CenterSpawnedObjects.IDNotSpawnedObjects; l < CenterSpawnedObjects.IDSpawnedObjects; l++)
            {
                if(CenterSpawnedObjects.Instance.ResourcesID[l] == 0)
                {
                    objectID = l;

                    CenterSpawnedObjects.Instance.ResourcesID[objectID] = 1;
                    CenterSpawnedObjects.Instance.ResourcesPositions[objectID] = FinalPoint;
                    CenterSpawnedObjects.Instance.ResourcesTypes[objectID] = i;

                    obj.ObjectID = objectID;
                    obj.Spawned = true;
                    obj.ID = i;
                    obj.Settings = AllID.Settings[i];
                    break;
                }
            }
        }
        else
            Debug.LogWarning("BiomeResourcesSpawner: Не удалось найти точку для спавна ресурса ID " + Types[i] + " в биоме ID " + ID);
    }
}