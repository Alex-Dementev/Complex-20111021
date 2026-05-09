using UnityEngine;

public class Menu : MonoBehaviour
{
    public Animator BlackScreen;
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
    }
}
