using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class UINavigation : MonoBehaviour
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

    public void MoveToScene(int sceneID) {
		SceneManager.LoadScene(sceneID);
	}

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    // These three functions are only for prototype, before level functionality takes care of this
    public void IncrementLevelsUnlocked()
    {
        int x = PlayerPrefs.GetInt("levelsUnlocked");
        PlayerPrefs.SetInt("levelsUnlocked", x + 1);
        Debug.Log("Levels Unlocked: " + PlayerPrefs.GetInt("levelsUnlocked"));
        UpdateLevelLockText();
    }
    
    public void DecrementLevelsUnlocked()
    {
        int x = PlayerPrefs.GetInt("levelsUnlocked");
        PlayerPrefs.SetInt("levelsUnlocked", x - 1);
        Debug.Log("Levels Unlocked: " + PlayerPrefs.GetInt("levelsUnlocked"));
        UpdateLevelLockText();
    }

    public void UpdateLevelLockText()
    {
        GameObject levelLockText = GameObject.Find("Display Level Lock");
        levelLockText.GetComponent<TMPro.TextMeshProUGUI>().text = "Levels Unlocked: " + PlayerPrefs.GetInt("levelsUnlocked");
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
