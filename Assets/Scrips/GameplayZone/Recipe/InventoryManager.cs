using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance
    public static InventoryManager Instance { get; private set; }
    private HashSet<string> unlockedElements = new HashSet<string>();

    [Header("Element Prefabs")]
    public GameObject elementPrefab;

    [Header("UI References")]
    public Transform inventoryContent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign the instance
            DontDestroyOnLoad(gameObject); // Ensure this instance persists across scenes (optional)
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void InitializeInventory(List<string> basicElements)
    {
        foreach (var elementName in basicElements)
        {
            AddElementToInventory(elementName);
            unlockedElements.Add(elementName);
        }
    }

    public void AddElementToInventory(string elementName)
    {
        SaveUnlockedElements();
        var newElement = Instantiate(elementPrefab, inventoryContent);
        var elementCollision = newElement.GetComponent<ElementCollision>();
        elementCollision.elementName = elementName;

        var rectTransform = newElement.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;

        var textMeshPro = newElement.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            // Optional: Ensure font is set (if necessary)
            if (textMeshPro.font == null)
            {
                var font = Resources.Load<TMP_FontAsset>("Fonts/Minecraft SDF");
                textMeshPro.font = textMeshPro.font = font;
            }

            // Set the text to the element name
            textMeshPro.text = elementName;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found in element prefab.");
        }
    }

    public void SaveUnlockedElements()
    {
        string path = Application.persistentDataPath + "/UnlockedElements.json";
        string json = JsonUtility.ToJson(new SerializableElementList(unlockedElements));
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"Unlocked elements saved to: {path}");
    }

    public void LoadUnlockedElements(List<string> basicElements)
    {
        string path = Application.persistentDataPath + "/UnlockedElements.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            var loadedElements = JsonUtility.FromJson<SerializableElementList>(json);
            unlockedElements.Clear();
            unlockedElements.UnionWith(loadedElements.elements);

            foreach (var elementName in unlockedElements)
            {
                if (!basicElements.Contains(elementName))
                {
                    AddElementToInventory(elementName);
                }
            }
            Debug.Log($"Unlocked elements loaded from: {path}");
        }
        else
        {
            Debug.LogWarning($"No unlocked elements file found at: {path}");
        }
    }
}
