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
    private void InitializeComponents()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        elementImage = GetComponent<Image>();
    }
    private void DetermineInitialParent()
    {
        isFromWorkspace = transform.parent.name == WorkspacePanelName;
    }
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
    private void SetDraggingState(bool isDragging)
    {
        canvasGroup.alpha = isDragging ? 0.6f : 1f;
        canvasGroup.blocksRaycasts = !isDragging;
    }
    private bool IsFromInventory()
    {
        return transform.parent.name == ContentPanelName && !isDraggingClone;
    }
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
    private void SetupClone(GameObject clone, PointerEventData eventData)
    {
        DraggableElement cloneDrag = clone.GetComponent<DraggableElement>();
        cloneDrag.isDraggingClone = true;
        cloneDrag.isFromWorkspace = true;

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        cloneRect.sizeDelta = rectTransform.sizeDelta;
        PositionCloneAtPointer(cloneRect, eventData);
    }
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
    private void MoveElement(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
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
}