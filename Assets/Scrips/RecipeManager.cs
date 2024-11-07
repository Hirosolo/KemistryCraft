using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject elementPrefab;

    [Header("Basic Elements")]
    public List<string> basicElements = new List<string>();

    [Header("UI References")]
    public Transform inventoryContent;
    public Transform workspacePanel;

    public HashSet<string> unlockedElements = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }

        LoadUnlockedElements();
    }

    private void InitializeInventory()
    {
        // Add basic elements to inventory
        foreach (string elementName in basicElements)
        {
            AddElementToInventory(elementName);
            unlockedElements.Add(elementName);
        }
    }

    public void AddElementToInventory(string elementName)
    {
        // Create new element in inventory
        GameObject newElement = Instantiate(elementPrefab, inventoryContent);

        // Set up the element
        ElementCollision elementCollision = newElement.GetComponent<ElementCollision>();
        elementCollision.elementName = elementName;

        // Set proper RectTransform values for inventory item
        RectTransform rectTransform = newElement.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;
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
                DiscoveredRecipesManager.Instance.AddDiscoveredRecipe(element1, element2, result);
                return true;
            }
        }

        return false;
    }

    private void SaveUnlockedElements()
    {
        string elementsJson = JsonUtility.ToJson(new SerializableElementList(unlockedElements));
        PlayerPrefs.SetString("UnlockedElements", elementsJson);
        PlayerPrefs.Save();
    }

    private void LoadUnlockedElements()
    {
        if (PlayerPrefs.HasKey("UnlockedElements"))
        {
            string elementsJson = PlayerPrefs.GetString("UnlockedElements");
            SerializableElementList loadedElements = JsonUtility.FromJson<SerializableElementList>(elementsJson);
            unlockedElements = new HashSet<string>(loadedElements.elements);

            // Recreate inventory elements
            foreach (string elementName in unlockedElements)
            {
                if (!basicElements.Contains(elementName))
                {
                    AddElementToInventory(elementName);
                }
            }
        }
    }
}

[System.Serializable]
public class SerializableElementList
{
    public List<string> elements;

    public SerializableElementList(HashSet<string> elementSet)
    {
        elements = new List<string>(elementSet);
    }
}