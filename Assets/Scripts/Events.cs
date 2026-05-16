using UnityEngine;
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour
{
    public void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "Main")
            SceneManager.LoadScene("Game");
        else if (SceneManager.GetActiveScene().name == "Game")
            SceneManager.LoadScene("Main");

        Debug.Log("Переход в другую сцену");
    }
}
