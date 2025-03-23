using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManu : MonoBehaviour
{


    public void  StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void EndGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
