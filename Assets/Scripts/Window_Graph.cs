using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite cirlceSprite;
    private RectTransform graphContainer;

    private void Awake(){
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        CreateCircle(new Vector2(100,100));

        // Get pairs
        List<Tuple<int, int>> pairs = genIndicies();

        // Print the generated pairs
        foreach (var pair in pairs)
        {
            // Console.WriteLine($"Pair: {pair.Item1}, {pair.Item2}");
            CreateCircle(new Vector2(pair.Item1, pair.Item2));
        }

    }

    private void CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = cirlceSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(20,20);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);


    }
    public List<Tuple<int,int>> genIndicies(){

        // Create a list to store random pairs
        List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();

        // Generate 20 random pairs
        for (int i = 0; i < 50; i++)
        {
            // Create a new Random instance for each pair (avoids correlation)
            System.Random random = new System.Random();

            // Generate two random numbers within a specific range (replace with your desired range)
            int firstNumber = random.Next(-435, 470);
            int secondNumber = random.Next(-150, 220);

            // Add the pair to the list
            pairs.Add(new Tuple<int, int>(firstNumber, secondNumber));
        }

        

        return pairs;
    }

}
