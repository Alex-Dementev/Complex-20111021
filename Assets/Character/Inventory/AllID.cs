using UnityEngine;

public class AllID : MonoBehaviour
{
    [Header("Объекты:")]
    public string[] Names;
    public string[] Descriptions;
    public Sprite[] Sprites;
    public Objects[] Prefab;
    public GameObject[] HandPrefab;
    public float[] Settings;
    public string[] SettingsStrings;
    public bool[] Buttery;
    public int[] SliderTypes;

    public const int Resources = 50;
    public const int Items = 100;
    public const int Ballons = 110;
    public const int Backpack = 115;
    public const int Butteries = 120;


    [Header("Постройки:")]
    public string[] BuildName;
    public string[] BuildDescription;
    public string[] BuildResource1;
    public string[] BuildResource2;
    public string[] BuildResource3;
    public string[] BuildResource4;
    public int[] BuildLevel;
    public int[] BuildScanned;
    public GameObject[] BuildPrefab;
    public GameObject[] PreviewPrefab;
    public Sprite[] BuildSprite;
}
