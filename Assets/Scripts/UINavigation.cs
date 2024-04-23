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

    // This function grabs the current level number from the LevelElements object and tries to load the next level... it does NOT check if the next level exists
    public void ToNextLevel()
    {
        try
        {
            int currentLevel = GameObject.Find("Level Elements").GetComponent<LevelBuilder>().GetLevelNumber();
            ToLevel(currentLevel + 1);
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Tried to load Next level but Level Elements not found");
            return;
        }
    }

    // This function checks whether the next level is unlocked, and removes the next level button if not
    public void CheckNextLevel()
    {
        // Debug.Log("Checking Next Level");
        try
        {
            int currentLevel = GameObject.Find("Level Elements").GetComponent<LevelBuilder>().GetLevelNumber();
            if (PlayerPrefs.GetInt("levelsUnlocked") <= currentLevel)
            {
                GameObject.Find("Next Level Button").SetActive(false);
                // Debug.Log("Next Level Button Disabled, level is not unlocked");
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Tried to check Next level but Level Elements or Next Level Button or another game object not found");
            return;
        }
    }

    // This function grabs the current level number from the LevelElements object and tries to load the previous level... it does NOT check if the previous level exists
    public void ToPreviousLevel()
    {
        try
        {
            int currentLevel = GameObject.Find("Level Elements").GetComponent<LevelBuilder>().GetLevelNumber();
            ToLevel(currentLevel - 1);
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Tried to load Previous Level but Level Elements not found");
            return;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
