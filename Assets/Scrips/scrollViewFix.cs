using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FastScrollView : MonoBehaviour
{
    private ScrollRect scrollRect;

    [Range(0.5f, 5f)]
    public float scrollSpeed = 2f; // Increase this value for faster scrolling

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        // Optimize ScrollRect settings for faster scrolling
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.5f; // Higher value = longer slide
        scrollRect.elasticity = 0.05f; // Lower value = less bounce
        scrollRect.scrollSensitivity = 25f; // Higher value = more sensitive to scroll input
        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        // Ensure vertical scroll is enabled
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
    }

    void Update()
    {
        // Make mousewheel scrolling faster
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector2 scrollPosition = scrollRect.normalizedPosition;
            scrollPosition.y += Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;
            scrollRect.normalizedPosition = scrollPosition;
        }
    }
}