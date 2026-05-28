using UnityEngine;

public class AllID : MonoBehaviour
{
    [Header("Объекты:")]
    public string[] Names;
    public string[] Descriptions;
    public Sprite[] Sprites;
    public Objects[] Prefab;
    public GameObject[] HandPrefab;

    [Header("Постройки:")]
    public string[] BuildName;
    public string[] BuildDescription;
    public string[] BuildResource1;
    public string[] BuildResource2;
    public string[] BuildResource3;
    public string[] BuildResource4;
    public int[] BuildLevel;
    public GameObject[] BuildPrefab;
    public Sprite[] BuildSprite;
}
