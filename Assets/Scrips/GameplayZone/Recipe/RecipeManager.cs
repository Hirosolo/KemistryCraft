using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; 
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }
    public List<string> unlockedElements = new List<string>();

    [Header("Basic Elements")]
    public List<string> basicElements = new List<string>();

    [Header("Recipes")]
    public List<Recipe> recipes = new List<Recipe>();

    private InventoryManager inventoryManager;

    [Header("Element Prefabs")]
    public GameObject elementPrefab;

    [Header("UI References")]
    public Transform inventoryContent;
    public Transform workspacePanel;

    [Header("Goal")]
    public TextMeshProUGUI goalText; 
    private string currentGoal; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inventoryManager = GetComponent<InventoryManager>();
            unlockedElements.AddRange(basicElements);
            inventoryManager.InitializeInventory(unlockedElements);
            LoadRecipes();
            GenerateNewGoal();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddRecipe(string element1, string element2, string result)
    {
        recipes.Add(new Recipe(element1, element2, result));
        SaveRecipes();
    }

    public bool TryCombine(string element1, string element2, out string result)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.Matches(element1, element2))
            {
                result = recipe.result;
                //DiscoveredRecipesManager.Instance.AddDiscoveredRecipe(element1, element2, result);
                AddElementToInventory(result);
                CheckGoal(result);
                return true;
            }
        }

        result = null;
        return false;
    }

    public void SaveRecipes()
    {
        string path = Application.persistentDataPath + "/Recipes.json";
        string json = JsonUtility.ToJson(new SerializableList<Recipe>(recipes)); // Uses the generic class for Recipe
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"Recipes saved to: {path}");
    }

    public void LoadRecipes()
    {
        string defaultPath = Application.dataPath + "/Recipes.json";
        string userPath = Application.persistentDataPath + "/Recipes.json";
        if (System.IO.File.Exists(defaultPath))
        {
            System.IO.File.Copy(defaultPath, userPath, overwrite: true);
            Debug.Log("Recipes file copied to persistent data path.");
            string json = System.IO.File.ReadAllText(userPath);
            SerializableList<Recipe> loadedRecipes = JsonUtility.FromJson<SerializableList<Recipe>>(json);
            recipes.Clear();
            recipes.AddRange(loadedRecipes.items);
            Debug.Log($"Recipes loaded from: {userPath}");
        }
        else
        {
            Debug.LogError("Default recipes file not found in game directory!");
            return;
        }
    }
    public void AddElementToInventory(string elementName)
    {
        // Check if the element is already unlocked
        if (!unlockedElements.Contains(elementName))
        {
            unlockedElements.Add(elementName);
            InventoryManager.Instance.AddElementToInventory(elementName);
            InventoryManager.Instance.SaveUnlockedElements();
            Debug.Log($"Added {elementName} to unlocked elements.");
        }
    }

    public void GenerateNewGoal()
    {
        List<string> possibleGoals = new List<string>();

        foreach (var recipe in recipes)
        {
            if (!basicElements.Contains(recipe.result) && !unlockedElements.Contains(recipe.result))
            {
                possibleGoals.Add(recipe.result);
            }
        }

        if (possibleGoals.Count == 0)
        {
            goalText.text = "Goal: None (All goals completed)";
            currentGoal = null;
            return;
        }

        currentGoal = possibleGoals[Random.Range(0, possibleGoals.Count)];
        goalText.text = $"Goal: {currentGoal}";
    }

    public void CheckGoal(string createdElement)
    {
        if (createdElement == currentGoal)
        {
            Debug.Log($"Goal achieved: {currentGoal}");
            GenerateNewGoal();
        }
    }

}
