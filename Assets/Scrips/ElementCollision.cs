using UnityEngine;
using UnityEngine.UI;

public class ElementCollision : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isOverlapping = false;
    private ElementCollision otherElement;

    [Header("Element Properties")]
    public string elementName;  // Name of this element (like "Water", "Fire", etc.)

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Only check for combinations if we're in the workspace
        if (transform.parent.name == "WorkspacePanel")
        {
            CheckForOverlap();
        }
    }

    void CheckForOverlap()
    {
        // Find all other ElementCollision components in the workspace
        ElementCollision[] elements = transform.parent.GetComponentsInChildren<ElementCollision>();

        foreach (ElementCollision other in elements)
        {
            // Skip checking against itself
            if (other == this)
                continue;

            // Check if they're overlapping
            if (IsOverlapping(other.GetComponent<RectTransform>()))
            {
                Debug.Log($"Overlap detected between {elementName} and {other.elementName}"); // Debug log
                TryCombineElements(other);
                break;
            }
        }
    }

    void TryCombineElements(ElementCollision other)
    {
        // Avoid multiple combinations in the same frame
        if (this == null || other == null)
            return;

        string result;
        if (RecipeManager.Instance.TryCombine(elementName, other.elementName, out result))
        {
            Debug.Log($"Recipe found! Combining {elementName} and {other.elementName} to create {result}"); // Debug log

            // Calculate middle position between the two elements
            Vector2 middlePosition = (rectTransform.anchoredPosition + other.rectTransform.anchoredPosition) / 2;

            // Create the new combined element
            RecipeManager.Instance.CreateNewElement(result, middlePosition);

            // Destroy the original elements
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    // Helper method to check if two RectTransforms are overlapping
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
}