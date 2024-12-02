using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiscoveredRecipesManager : MonoBehaviour
{
    public static DiscoveredRecipesManager Instance { get; private set; }
    private HashSet<string> discoveredRecipes = new HashSet<string>();
    [Header("UI References")]
    public GameObject recipeListPanel;
    public GameObject recipeEntryPrefab;
    public Transform recipeListContent;

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
    public void AddDiscoveredRecipe(string element1, string element2, string result)
    {
        string recipeKey = GetRecipeKey(element1, element2);

        if (!discoveredRecipes.Contains(recipeKey))
        {
            discoveredRecipes.Add(recipeKey);
            //CreateRecipeEntry(element1, element2, result);
            SaveDiscoveredRecipes();
        }
    }
    private string GetRecipeKey(string element1, string element2)
    {
        // Create a consistent key regardless of element order
        var elements = new[] { element1, element2 };
        System.Array.Sort(elements);
        return string.Join("+", elements);
    }
    private void CreateRecipeEntry(string element1, string element2, string result)
    {
        GameObject entry = Instantiate(recipeEntryPrefab, recipeListContent);
        TextMeshProUGUI recipeText = entry.GetComponentInChildren<TextMeshProUGUI>();
        recipeText.text = $"{element1} + {element2} = {result}";
    }
    private void SaveDiscoveredRecipes()
    {
        // Use the generic SerializableList class for saving
        string recipesJson = JsonUtility.ToJson(new SerializableList<string>(discoveredRecipes));
        PlayerPrefs.SetString("DiscoveredRecipes", recipesJson);
        PlayerPrefs.Save();
    }
    private void SaveDiscoveredRecipesToFile()
    {
        string path = Application.persistentDataPath + "/DiscoveredRecipes.json";
        string recipesJson = JsonUtility.ToJson(new SerializableList<string>(discoveredRecipes));
        System.IO.File.WriteAllText(path, recipesJson);
        Debug.Log("Recipes saved to: " + path);
    }
    private void LoadDiscoveredRecipes()
    {
        if (PlayerPrefs.HasKey("DiscoveredRecipes"))
        {
            string recipesJson = PlayerPrefs.GetString("DiscoveredRecipes");
            SerializableList<string> loadedRecipes = JsonUtility.FromJson<SerializableList<string>>(recipesJson);
            discoveredRecipes = new HashSet<string>(loadedRecipes.items);

            // Recreate UI entries for loaded recipes
            RecreateRecipeEntries();
        }
    }
    private void LoadDiscoveredRecipesFromFile()
    {
        string path = Application.persistentDataPath + "/DiscoveredRecipes.json";
        if (System.IO.File.Exists(path))
        {
            string recipesJson = System.IO.File.ReadAllText(path);
            SerializableList<string> loadedRecipes = JsonUtility.FromJson<SerializableList<string>>(recipesJson);
            discoveredRecipes = new HashSet<string>(loadedRecipes.items);

            Debug.Log("Recipes loaded from: " + path);

            RecreateRecipeEntries();
        }
        else
        {
            Debug.LogWarning("No recipe file found at: " + path);
        }
    }
    private void RecreateRecipeEntries()
    {
        foreach (string recipeKey in discoveredRecipes)
        {
            string[] elements = recipeKey.Split('+');
            if (elements.Length == 2)
            {
                string result;
                if (RecipeManager.Instance.TryCombine(elements[0], elements[1], out result))
                {
                    CreateRecipeEntry(elements[0], elements[1], result);
                }
            }
        }
    }
}

[System.Serializable]
public class SerializableRecipeList
{
    public List<string> recipes;

    public SerializableRecipeList(HashSet<string> recipeSet)
    {
        recipes = new List<string>(recipeSet);
    }
}