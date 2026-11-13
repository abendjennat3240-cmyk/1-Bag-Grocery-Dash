// Controls the picking up and dragging of groceries
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;
    private TargetJoint2D dragJoint;
    private int groceryLayerMask;

    // Whether the grocery is currently being dragged
    public bool IsDragging
    {
        get;
        private set;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        groceryLayerMask = LayerMask.GetMask("Default"); // Only detects objects on the default layer
    }
    // Mouse button is being clicked to drag the grocery
    void OnMouseDown()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, groceryLayerMask);
        // Return if this grocery is not being clicked
        if (hit.collider == null || hit.collider.gameObject != gameObject)
        {
            return;
        }
        rb.WakeUp();
        rb.constraints = RigidbodyConstraints2D.None; // Allows the grocery to move in any way while being dragged
        IsDragging = true;
        AudioManager.Instance.PlayPickup(); // Audio effect
        dragJoint = gameObject.AddComponent<TargetJoint2D>();
        dragJoint.autoConfigureTarget = false;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(transform.position.z - mainCamera.transform.position.z)));
        dragJoint.anchor = transform.InverseTransformPoint(worldPosition);
        dragJoint.target = rb.position;
        dragJoint.maxForce = 1000f; // Maximum force set to refrain the grocery from being dragged with too much force which led to issues on release
        dragJoint.dampingRatio = 1f; // Sets how quickly movement is settled
        dragJoint.frequency = 15f; // Caps how strongly movement is responded to so the grocery does not fly off the screen on release
    }
    // Mouse button is released and grocery is no longer being dragged
    void OnMouseUp()
    {
        IsDragging = false;
        // If a drag joint exists, it is removed so the mouse is not connected to the grocery anymore
        if (dragJoint != null)
        {
            Destroy(dragJoint);
            dragJoint = null;
        }
        rb.WakeUp(); // Resumes grocery physics
    }
    // Update is called once per frame
    void Update()
    {
        // Calculates and updates the drag position if the grocery is being dragged successfully
        if (IsDragging && dragJoint != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            dragJoint.target = new Vector2(worldPosition.x, worldPosition.y);
        }
    }
}
