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
    }

    public void ToLevel(int levelNumber) {
		// SceneManager.LoadSceneAsync(sceneID);
		SceneManager.LoadSceneAsync("Level " + levelNumber);
	}

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
