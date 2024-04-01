using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    private static int levelsUnlocked = 1; // The initial value of levels to be unlocked
    public static string levelsUnlockedKey = "levelsUnlocked";
    [SerializeField] private GameObject lineControllerPrefab;
    private GameObject[] levelNodes;
    private GameObject[] connectionLines;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(levelsUnlockedKey))
        {
            levelsUnlocked = PlayerPrefs.GetInt(levelsUnlockedKey);
        }
        else
        {
            PlayerPrefs.SetInt(levelsUnlockedKey, levelsUnlocked);
        }
        DrawConnections(); // this function also initializes levelNodes and connectionLines
        Debug.Assert(levelsUnlocked > 0 && levelsUnlocked <= levelNodes.Length);
        DrawAttacks();
    }

    // Draw attacks between level nodes that have already been unlocked
    private void DrawAttacks()
    {
        Debug.Assert(connectionLines != null);
        for (int i = 0; i < levelsUnlocked - 1; i++) // - 1 to exclude the last level, + 1 to exclude the node for home button connection
        {
            levelNodes[i].GetComponent<SpriteRenderer>().color = Color.red;
            connectionLines[i].GetComponent<LineController>().StartAttack(levelNodes[i].transform, Color.red, 1.0f);
        }
        // This is the connection between the last unlocked level and the first locked level
        // So, we only color half of the attack
        levelNodes[levelsUnlocked - 1].GetComponent<SpriteRenderer>().color = Color.red;
        // If all of the levels are  unlocked, there is no "next level" to attack
        if (levelsUnlocked == levelNodes.Length) return;
        connectionLines[levelsUnlocked - 1].GetComponent<LineController>().StartAttack(levelNodes[levelsUnlocked - 1].transform, Color.red, 0.5f);
    }

    // Draw connections between level nodes
    // This function also initializes the levelNodes and connectionLines arrays
    private void DrawConnections()
    {
        // Get references to the level nodes
        GameObject canvas = GameObject.Find("Canvas");
        int levelNodeCount = canvas.transform.childCount - 1; // -1 to exclude the Home Button
        levelNodes = new GameObject[levelNodeCount];
        for (int i = 0; i < levelNodeCount; i++)
        {
            levelNodes[i] = canvas.transform.GetChild(i + 1).gameObject;
        }
        connectionLines = new GameObject[levelNodeCount];
        // Each level node should have a connection to the next level node
        for (int i = 0; i < levelNodeCount - 1; i++)
        {
            GameObject newConnection = Instantiate(lineControllerPrefab, transform.position, transform.rotation, transform);
            newConnection.GetComponent<LineController>().DrawBackgroundSegment(levelNodes[i].transform, levelNodes[i + 1].transform);
            connectionLines[i] = newConnection;
        }
    }

    // Check if a level is unlocked
    public static bool IsLevelUnlocked(int levelNumber) {
        return (levelNumber <= levelsUnlocked && levelNumber > 0);
    }
}
