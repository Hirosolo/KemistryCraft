using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string element1;
    public string element2;
    public string result;
}

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("Recipe List")]
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Element Prefabs")]
    public GameObject elementPrefab;  // Reference to your base ElementPrefab

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TryCombine(string element1, string element2, out string result)
    {
        result = null;

        Debug.Log($"Checking recipe for {element1} + {element2}"); // Debug log

        foreach (Recipe recipe in recipes)
        {
            if ((recipe.element1 == element1 && recipe.element2 == element2) ||
                (recipe.element1 == element2 && recipe.element2 == element1))
            {
                result = recipe.result;
                Debug.Log($"Recipe found! Result: {result}"); // Debug log
                return true;
            }
        }

        Debug.Log("No matching recipe found"); // Debug log
        return false;
    }

    public GameObject CreateNewElement(string elementName, Vector2 position)
    {
        Debug.Log($"Creating new element: {elementName} at position: {position}"); // Debug log

        // Find the workspace panel
        Transform workspacePanel = GameObject.Find("WorkspacePanel").transform;

        // Instantiate the element at the given position
        GameObject newElement = Instantiate(elementPrefab, workspacePanel);

        // Set up the RectTransform
        RectTransform rectTransform = newElement.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        // Set the element name
        ElementCollision elementCollision = newElement.GetComponent<ElementCollision>();
        elementCollision.elementName = elementName;

        return newElement;
    }
}