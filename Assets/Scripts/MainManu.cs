using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManu : MonoBehaviour
{


    public void  StartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void EndGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
