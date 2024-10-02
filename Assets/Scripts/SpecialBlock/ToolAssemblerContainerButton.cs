using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class HoverMoveImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public float moveSpeed = 15f;  // Speed of the movement
    private bool isHovering = false;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float moveDistance = 23f;  // How far to move
    public GameObject container;

    void Start()
    {
        // Store the initial position of the image
        initialPosition = transform.localPosition;
        targetPosition = initialPosition - new Vector3(moveDistance, 0, 0);  // 50 pixels to the left
    }

    void Update()
    {
        // If the image is being hovered over, move toward the target position
        if (isHovering)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
        }
        else
        {
            // Move back to the initial position if not hovering
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * moveSpeed);
        }
    }

    // Detect when the mouse enters the image area
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    // Detect when the mouse leaves the image area
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.GetComponentInParent<ToolAssemblerManager>().ChangeCurrentContainer(container);
    }
}
