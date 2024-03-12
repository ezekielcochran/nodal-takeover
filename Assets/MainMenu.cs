using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ToLevelsMenu()
    {
        SceneManager.LoadSceneAsync("Level Select");
        // Load the next scene in the build settings
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
