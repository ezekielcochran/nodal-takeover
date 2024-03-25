using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
 
    // }

    public void ToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
        Debug.Log("Main Menu");
    }

    public void ToLevel(int levelNumber) {
		// SceneManager.LoadSceneAsync(sceneID);
		SceneManager.LoadSceneAsync("Level " + levelNumber);
	}

    // Seems like a tacky way to do this... please fix later, this is just for prototype
    public void ToLevelsMenu() {
        SceneManager.LoadSceneAsync("Level Select");
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
