using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image[] ImageSlots;
    [HideInInspector]public int TotalSlots = 5;
    [HideInInspector]public int[] IndexSlots;
    public GameObject[] Slots;
    public Image[] SlotsAllocations;
    public Text Name;
    public Text Description;
    public AllID AllID;
    private int CurrentSlot;


    private void Start()
    {
        TotalSlots = 6;
        IndexSlots = new int[TotalSlots];

        for(int i = 0; i < Slots.Length; i++)
        {
            if(i <= TotalSlots - 1)
                Slots[i].SetActive(true);
            else
                Slots[i].SetActive(false);

        }
        

        string data = PlayerPrefs.GetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), "");
        string[] split = data.Split('|');

        for (int i = 0; i < IndexSlots.Length; i++)
        {
            if (i < split.Length && split[i] != "")
                IndexSlots[i] = int.Parse(split[i]);
            else
                IndexSlots[i] = 0;
        }


        Description.text = "";
        Name.text = "";
    }

    void Update()
    {
        for(int i = 0; i < TotalSlots; i++)
        {
            if(IndexSlots[i] == 1)
                ImageSlots[i].color = new Color(63f/255f, 63f/255f, 63f/255f, 1f);
            else if(IndexSlots[i] == 2)
                ImageSlots[i].color = new Color(200f/255f, 169f/255f, 97f/255f, 1f);
        }
    }

    public void ClickToSlot(int Index)
    {
        SlotsAllocations[CurrentSlot].color = new Color(55f/255f, 55f/255f, 55f/255f);

        Description.text = AllID.Descriptions[IndexSlots[Index]];
        Name.text = AllID.Names[IndexSlots[Index]];

        if(Index != CurrentSlot)
            SlotsAllocations[Index].color = new Color(85f/255f, 85f/255f, 85f/255f);

        CurrentSlot = Index;
    }

    public void Save()
    {
        string data = string.Join("|", IndexSlots);
        PlayerPrefs.SetString("InventorySlots" + PlayerPrefs.GetInt("WorldIndex", 0), data);
    }
}
