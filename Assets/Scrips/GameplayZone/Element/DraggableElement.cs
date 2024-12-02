using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Image elementImage;

    private bool isDraggingClone = false;
    private bool isFromWorkspace = false;

    private const string WorkspacePanelName = "WorkspacePanel";
    private const string ContentPanelName = "Content";
    private const string InventoryScrollViewName = "Scroll View";

    private void Awake()
    {
        InitializeComponents();
        DetermineInitialParent();
    }

    private void Start()
    {
        StoreOriginalPosition();
    }

    // Initializes essential components
    private void InitializeComponents()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        elementImage = GetComponent<Image>();
    }
    // Determines if the element originates from the workspace.
    private void DetermineInitialParent()
    {
        isFromWorkspace = transform.parent.name == WorkspacePanelName;
    }

    // Stores the original position of the element
    private void StoreOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetDraggingState(true);

        if (IsFromInventory())
        {
            CreateAndDragClone(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveElement(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggingState(false);

        if (isFromWorkspace)
        {
            HandleDrop(eventData);
        }
    }

    // Adjusts the element's visual state when dragging starts or ends.
    private void SetDraggingState(bool isDragging)
    {
        canvasGroup.alpha = isDragging ? 0.6f : 1f;
        canvasGroup.blocksRaycasts = !isDragging;
    }

    // Checks if the element is from the inventory panel.
    private bool IsFromInventory()
    {
        return transform.parent.name == ContentPanelName && !isDraggingClone;
    }

    // Creates a clone of the element and initiates its drag operation.
    private void CreateAndDragClone(PointerEventData eventData)
    {
        Transform workspacePanel = GameObject.Find(WorkspacePanelName).transform;

        GameObject clone = Instantiate(gameObject, workspacePanel);
        SetupClone(clone, eventData);

        // Reset original element's state
        SetDraggingState(false);

        // Transfer drag operation to the clone
        eventData.pointerDrag = clone;
    }

    // Configures a cloned element for dragging.
    private void SetupClone(GameObject clone, PointerEventData eventData)
    {
        DraggableElement cloneDrag = clone.GetComponent<DraggableElement>();
        cloneDrag.isDraggingClone = true;
        cloneDrag.isFromWorkspace = true;

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.sizeDelta = rectTransform.sizeDelta;
        PositionCloneAtPointer(cloneRect, eventData);
    }

    // Positions the clone at the pointer's location in the workspace.
    private void PositionCloneAtPointer(RectTransform cloneRect, PointerEventData eventData)
    {
        Transform workspacePanel = GameObject.Find(WorkspacePanelName).transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            workspacePanel as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        cloneRect.anchoredPosition = localPoint;
        cloneRect.anchorMin = new Vector2(0.5f, 0.5f);
        cloneRect.anchorMax = new Vector2(0.5f, 0.5f);
        cloneRect.pivot = new Vector2(0.5f, 0.5f);
    }

    // Handles the drag operation by moving the element.
    private void MoveElement(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Handles the drop action when dragging ends.
    private void HandleDrop(PointerEventData eventData)
    {
        if (IsDroppedInInventory(eventData.position))
        {
            Destroy(gameObject);
        }
        else
        {
            ClampPositionWithinWorkspace();
        }
    }

    // Checks if the element is dropped in the inventory.
    private bool IsDroppedInInventory(Vector2 position)
    {
        GameObject inventoryScrollView = GameObject.Find(InventoryScrollViewName);
        if (inventoryScrollView != null)
        {
            RectTransform scrollViewRect = inventoryScrollView.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                scrollViewRect,
                position,
                null,
                out Vector2 localPoint))
            {
                return scrollViewRect.rect.Contains(localPoint);
            }
        }
        return false;
    }

    // Clamps the element's position within the workspace boundaries.

    private void ClampPositionWithinWorkspace()
    {
        RectTransform workspaceRect = transform.parent.GetComponent<RectTransform>();
        Vector2 clampedPosition = rectTransform.anchoredPosition;

        float halfWidth = rectTransform.rect.width / 2;
        float halfHeight = rectTransform.rect.height / 2;

        float maxX = workspaceRect.rect.width / 2 - halfWidth;
        float maxY = workspaceRect.rect.height / 2 - halfHeight;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -maxY, maxY);

        rectTransform.anchoredPosition = clampedPosition;
    }

    public void OnClick(PointerEventData eventData)
    {

    }
}