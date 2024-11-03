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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TryCombine(string element1, string element2, out string result)
    {
        result = null;

        foreach (Recipe recipe in recipes)
        {
            if ((recipe.element1 == element1 && recipe.element2 == element2) ||
                (recipe.element1 == element2 && recipe.element2 == element1))
            {
                result = recipe.result;
                return true;
            }
        }

        return false;
    }

    public GameObject CreateNewElement(string elementName, Vector2 position)
    {
        // Instantiate the element at the given position
        GameObject newElement = Instantiate(elementPrefab, GameObject.Find("WorkspacePanel").transform);

        // Set up the RectTransform
        RectTransform rectTransform = newElement.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        // Set the element name
        ElementCollision elementCollision = newElement.GetComponent<ElementCollision>();
        elementCollision.elementName = elementName;

        // You might want to set different sprites/colors for different elements
        // This can be expanded later

        return newElement;
    }
}