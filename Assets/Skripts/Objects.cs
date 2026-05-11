using UnityEngine;

public class Objects : MonoBehaviour
{
    public int ID;
    public int ObjectID;
    public string Name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int[] ResourcesID = new int[500];
    public void LoadResourcesID()
    {
        string data = PlayerPrefs.GetString("ResourcesID" + PlayerPrefs.GetInt("WorldIndex", 0), "");

        if (string.IsNullOrEmpty(data))
        {
            ResourcesID = new int[500];
            return;
        }

        string[] split = data.Split('|');

        for (int i = 0; i < ResourcesID.Length; i++)
        {
            if (i < split.Length)
                ResourcesID[i] = int.Parse(split[i]);
            else
                ResourcesID[i] = 0;
        }
    }
    void Start()
    {
        LoadResourcesID();

        if(ResourcesID[ObjectID] == 1)
            Destroy(gameObject);

        switch (ID)
        {
            case 1:
                Name = "Песчаник";
                break;
            case 2:
                Name = "Уголь";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
