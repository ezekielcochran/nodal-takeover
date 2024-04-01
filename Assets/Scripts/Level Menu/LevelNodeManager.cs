using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNodeManager : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    private bool unlocked;
    private TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
        // Fake level node, such as for showing connection to the home button
        if (levelNumber < 0)
        {
            unlocked = false;
            return;
        }
        unlocked = LevelMenuManager.IsLevelUnlocked(levelNumber);
        // Debug.Log("Level " + levelNumber + " is initially unlocked: " + unlocked); // I'd like this to return true but this script seems to run before LevelMenuManager.cs

        levelText = GetComponentInChildren<TextMeshProUGUI>();
        levelText.text = levelNumber.ToString();
        // To keep the text appropriately sized within the node
        if (levelNumber >= 10) {
            levelText.fontSize = levelText.fontSize * 0.70f;
        }
        
    }

    void OnMouseDown() {
        unlocked = LevelMenuManager.IsLevelUnlocked(levelNumber);
        Debug.Log("Level " + levelNumber + " is unlocked: " + unlocked);
        if (unlocked) {
            UINavigation uiNavigation = GameObject.Find("Canvas").GetComponent<UINavigation>();
            uiNavigation.ToLevel(levelNumber);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
