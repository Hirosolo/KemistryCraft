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
        if (isOverlapping && otherElement != null)
        {
            // Check if elements are close enough to combine
            float distance = Vector2.Distance(rectTransform.anchoredPosition, otherElement.rectTransform.anchoredPosition);

            // You can adjust this threshold based on your element size
            if (distance < 100f)
            {
                TryCombineElements(otherElement);
            }
        }
    }

    void OnRectTransformOverlap(RectTransform other)
    {
        ElementCollision otherElementScript = other.GetComponent<ElementCollision>();
        if (otherElementScript != null && otherElementScript != this)
        {
            isOverlapping = true;
            otherElement = otherElementScript;
        }
    }
    void TryCombineElements(ElementCollision other)
    {
        string result;
        if (RecipeManager.Instance.TryCombine(elementName, other.elementName, out result))
        {
            // Calculate middle position between the two elements
            Vector2 middlePosition = (rectTransform.anchoredPosition + other.rectTransform.anchoredPosition) / 2;

            // Create the new combined element
            RecipeManager.Instance.CreateNewElement(result, middlePosition);

            // Destroy the original elements
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        // Reset overlap state
        isOverlapping = false;
        otherElement = null;
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