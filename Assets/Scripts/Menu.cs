using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Menu : MonoBehaviour
{
    public Animator BlackScreen;
    public Text[] TimeTexts;
    private float GameTimeSecond;
    private float GameTimeHours;
    private int WorldIndexDelete;

    public Text[] StartText;

    public Animator AnimatorDeletePanel;
    public Text DeleteText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadWorld1()
    {
        PlayerPrefs.SetInt("WorldIndex", 0);
        BlackScreen.Play("End");
    }
    public void LoadWorld2()
    {
        PlayerPrefs.SetInt("WorldIndex", 1);
        BlackScreen.Play("End");
    }
    public void LoadWorld3()
    {
        PlayerPrefs.SetInt("WorldIndex", 2);
        BlackScreen.Play("End");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        Time.timeScale = 1;

        GameTimeSecond = PlayerPrefs.GetFloat("AllTime0", 0);
        if(GameTimeSecond == 0)
            StartText[0].text = "Начать";
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[0].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";
        GameTimeSecond = PlayerPrefs.GetFloat("AllTime1", 0);
        if(GameTimeSecond == 0)
            StartText[1].text = "Начать";
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[1].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";
        GameTimeSecond = PlayerPrefs.GetFloat("AllTime2", 0);
        if(GameTimeSecond == 0)
            StartText[2].text = "Начать";
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[2].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";
    }

    public void DeleteWorld1()
    {
        WorldIndexDelete = 0;
        DeleteText.text = "Удалить мир 1?";
        AnimatorDeletePanel.CrossFade("Start", 0.2f);
    }
    public void DeleteWorld2()
    {
        WorldIndexDelete = 1;
        DeleteText.text = "Удалить мир 2?";
        AnimatorDeletePanel.CrossFade("Start", 0.2f);
    }
    public void DeleteWorld3()
    {
        WorldIndexDelete = 2;
        DeleteText.text = "Удалить мир 3?";
        AnimatorDeletePanel.CrossFade("Start", 0.2f);
    }

    public void DeleteWorldYes()
    {
        PlayerPrefs.DeleteKey("Oxygen" + WorldIndexDelete);
        PlayerPrefs.DeleteKey("playerData" + WorldIndexDelete);
        PlayerPrefs.DeleteKey("AllTime" + WorldIndexDelete);
        PlayerPrefs.DeleteKey("Heals" + WorldIndexDelete);
        PlayerPrefs.DeleteKey("RevivePosition" + WorldIndexDelete);
        PlayerPrefs.DeleteKey("InventorySlots" + WorldIndexDelete);


        StartText[WorldIndexDelete].text = "Начать";


        // Получаем путь к файлу текущего мира (на основе PlayerPrefs "WorldIndex")
        string fileName = $"World_{WorldIndexDelete}_Data.txt";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        // Проверяем, есть ли такой файл, и сносим его
        if (File.Exists(path))
            File.Delete(path);


        GameTimeSecond = PlayerPrefs.GetFloat("AllTime0", 0);
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[0].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";
        GameTimeSecond = PlayerPrefs.GetFloat("AllTime1", 0);
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[1].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";
        GameTimeSecond = PlayerPrefs.GetFloat("AllTime2", 0);
        GameTimeHours = (int)(GameTimeSecond / 3600);
        TimeTexts[2].text = GameTimeHours + " ч  " + (int)((GameTimeSecond - (GameTimeHours * 3600)) / 60) + " мин";

        AnimatorDeletePanel.CrossFade("End", 0.2f);
    }
    public void DeleteWorldNo()
    {
        AnimatorDeletePanel.CrossFade("End", 0.2f);
    }
}
