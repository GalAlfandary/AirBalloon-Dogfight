using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }


    public void ExitGame()
    {

        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
