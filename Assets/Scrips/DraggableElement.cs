using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Image image;
    private bool isDraggingClone = false;
    private bool isFromWorkspace = false;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Set isFromWorkspace based on initial parent
        isFromWorkspace = transform.parent.name == "WorkspacePanel";
    }

    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Check if this is an element in the Content (inventory) that needs to be cloned
        if (transform.parent.name == "Content" && !isDraggingClone)
        {
            // Find the workspace panel
            Transform workspacePanel = GameObject.Find("WorkspacePanel").transform;

            // Create the copy as a child of the workspace panel
            GameObject copy = Instantiate(gameObject, workspacePanel);

            // Get the RectTransform of the copy
            RectTransform copyRect = copy.GetComponent<RectTransform>();

            // Set the proper anchors and pivot
            copyRect.anchorMin = new Vector2(0.5f, 0.5f);
            copyRect.anchorMax = new Vector2(0.5f, 0.5f);
            copyRect.pivot = new Vector2(0.5f, 0.5f);

            // Convert screen position to local position in the workspace panel
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                workspacePanel as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint);

            // Set the position
            copyRect.anchoredPosition = localPoint;

            // Setup the clone
            DraggableElement copyDrag = copy.GetComponent<DraggableElement>();
            copyDrag.isDraggingClone = true;
            copyDrag.isFromWorkspace = true;  // Mark as workspace element

            // Ensure the copy has the same size as the original
            copyRect.sizeDelta = rectTransform.sizeDelta;

            // Transfer the drag operation to the clone
            eventData.pointerDrag = copy;

            // Reset the original element
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Only process if this is an element from the workspace
        if (isFromWorkspace)
        {
            // Get the mouse position
            Vector2 mousePosition = eventData.position;

            // Find the inventory scroll view
            GameObject scrollView = GameObject.Find("Scroll View");
            if (scrollView != null)
            {
                RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    scrollViewRect,
                    mousePosition,
                    eventData.pressEventCamera,
                    out localPoint))
                {
                    // Check if the point is inside the scroll view rect
                    if (scrollViewRect.rect.Contains(localPoint))
                    {
                        // Destroy the element if it's dropped on the inventory
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            // If not destroyed, clamp position within workspace
            if (transform.parent.name == "WorkspacePanel")
            {
                RectTransform workspaceRect = transform.parent.GetComponent<RectTransform>();
                Vector2 localPos = rectTransform.anchoredPosition;

                // Calculate the bounds
                float halfWidth = rectTransform.rect.width / 2;
                float halfHeight = rectTransform.rect.height / 2;
                float maxX = workspaceRect.rect.width / 2 - halfWidth;
                float maxY = workspaceRect.rect.height / 2 - halfHeight;

                // Clamp the position
                localPos.x = Mathf.Clamp(localPos.x, -maxX, maxX);
                localPos.y = Mathf.Clamp(localPos.y, -maxY, maxY);

                rectTransform.anchoredPosition = localPos;
            }
        }
    }
}