using UnityEngine;

public class AllTimeInGame : MonoBehaviour
{
    public float AllTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AllTime = PlayerPrefs.GetFloat("AllTime" + PlayerPrefs.GetInt("WorldIndex"), 0);
    }

    // Update is called once per frame
    void Update()
    {
        AllTime += Time.unscaledDeltaTime;
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("AllTime" + PlayerPrefs.GetInt("WorldIndex"), AllTime);
    }
}
