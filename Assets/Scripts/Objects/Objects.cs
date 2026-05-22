using UnityEngine;

public class Objects : MonoBehaviour
{
    public int ID;
    public int ObjectID;
    public string Name;
    public AllID AllID;
    public CenterSpawnedObjects CenterSpawnedObjects;
    private float IsDelay;
    private float TimeIsDelay;
    public bool Spawned;


    void Start()
    {
        Invoke("UpdateState", 0.15f);

        Name = AllID.Names[ID];

        TimeIsDelay = Random.Range(1.5f, 2.5f);
        IsDelay = TimeIsDelay;
    }

    // Update is called once per frame
    void Update()
    {
        IsDelay -= Time.deltaTime;

        if(IsDelay <= 0)
        {
            IsDelay = TimeIsDelay;
            CenterSpawnedObjects.ResourcesPositions[ObjectID] = transform.position;
            CenterSpawnedObjects.ResourcesRotations[ObjectID] = transform.eulerAngles;
        }
    }

    public void DestroyObject()
    {
        if(!Spawned)
        {
            CenterSpawnedObjects.ResourcesID[ObjectID] = 1;
            CenterSpawnedObjects.ResourcesPositions[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.ResourcesRotations[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.ResourcesTypes[ObjectID] = 0;
        }
        else
        {
            CenterSpawnedObjects.ResourcesID[ObjectID] = 0;
            CenterSpawnedObjects.ResourcesPositions[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.ResourcesRotations[ObjectID] = new Vector3(0, 0, 0);
            CenterSpawnedObjects.ResourcesTypes[ObjectID] = 0;
        }

        Destroy(gameObject);
    }

    public void UpdateState()
    {
        if(CenterSpawnedObjects.ResourcesID[ObjectID] == 1 && !Spawned)
        {
            Destroy(gameObject);
            return;
        }

        if(CenterSpawnedObjects.ResourcesPositions[ObjectID] != new Vector3(0, 0, 0))
        {
            transform.position = CenterSpawnedObjects.ResourcesPositions[ObjectID];
            transform.rotation = Quaternion.Euler(CenterSpawnedObjects.ResourcesPositions[ObjectID]);
        }
    }
}
