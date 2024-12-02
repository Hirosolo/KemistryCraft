using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementCollision : MonoBehaviour
{
    private RectTransform rectTransform;
    private RecipeManager recipeManager;

    [Header("Element Properties")]
    public string elementName;

    [Header("UI References")]
    public TextMeshProUGUI elementLabel;

    private bool isBeingDestroyed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        recipeManager = RecipeManager.Instance;

        RectTransform labelRect = elementLabel.GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = elementLabel.GetComponent<TextMeshProUGUI>();
        // Set the anchor to stretch in both directions
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        // Reset position and size
        labelRect.localScale = Vector2.one;

        elementLabel = elementLabel.GetComponent<TextMeshProUGUI>();
        elementLabel.fontStyle = FontStyles.Bold;
        elementLabel.fontSize = 18;
        elementLabel.enableWordWrapping = false;
        elementLabel.overflowMode = TextOverflowModes.Overflow;
    }

    void Start()
    {
        UpdateElementLabel();
    }

    void Update()
    {
        if (transform.parent != null && transform.parent.name == "WorkspacePanel" && !isBeingDestroyed)
        {
            CheckForOverlap();
        }
    }

    void CheckForOverlap()
    {
        ElementCollision[] elements = transform.parent.GetComponentsInChildren<ElementCollision>();

        foreach (ElementCollision other in elements)
        {
            if (other == this || other.isBeingDestroyed)
                continue;

            if (IsOverlapping(other.GetComponent<RectTransform>()))
            {
                TryCombineElements(other);
                break;
            }
        }
    }

    void TryCombineElements(ElementCollision other)
    {
        string result;
        if (recipeManager.TryCombine(elementName, other.elementName, out result))
        {
            Vector2 middlePosition = (rectTransform.anchoredPosition + other.rectTransform.anchoredPosition) / 2;

            // Mark elements for destruction to prevent further combinations
            isBeingDestroyed = true;
            other.isBeingDestroyed = true;

            // Create the new combined element
            CreateNewElement(result, middlePosition);

            // Destroy the original elements
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void CreateNewElement(string elementName, Vector2 position)
    {
        GameObject newElement = Instantiate(recipeManager.elementPrefab, recipeManager.workspacePanel);

        // Set up the element
        ElementCollision elementCollision = newElement.GetComponent<ElementCollision>();
        elementCollision.elementName = elementName;

        // Set position in workspace
        RectTransform newElementRect = newElement.GetComponent<RectTransform>();
        newElementRect.anchoredPosition = position;

        // If this is a new discovery, add to inventory and notify DiscoveredRecipesManager
        if (!recipeManager.unlockedElements.Contains(elementName))
        {
            recipeManager.AddElementToInventory(elementName);
            DiscoveredRecipesManager.Instance.AddDiscoveredRecipe(elementName, elementName, elementName);
            Debug.Log($"New element discovered: {elementName}");
        }
    }

    public bool IsOverlapping(RectTransform other)
    {
        Rect rect1 = GetWorldSpaceRect(rectTransform);
        Rect rect2 = GetWorldSpaceRect(other);

        return rect1.Overlaps(rect2);
    }

    private Rect GetWorldSpaceRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = corners[0];
        Vector2 max = corners[2];

        return new Rect(min, max - min);
    }

    private void UpdateElementLabel()
    {
        elementLabel.text = elementName;
    }
}