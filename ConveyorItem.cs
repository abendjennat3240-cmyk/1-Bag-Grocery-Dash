// Handles the passive movement of the conveyor belt as well as collisions
using UnityEngine;
using System.Collections.Generic;

public class ConveyorItem : MonoBehaviour
{
    public float speed = 4f; // Conveyor belt speed
    private Rigidbody2D rb;
    private Draggable draggable;
    private bool isOnConveyor; // Whether a grocery is touching the conveyor belt
    private HashSet<ConveyorItem> touchingItems = new HashSet<ConveyorItem>(); // All items currently touching the conveyor belt
    public bool IsTouchingAnotherItem => touchingItems.Count > 0; // Whether the grocery touching the left side of the bag at the end of the conveyor belt is also touching another item, for countdown purposes
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        draggable = GetComponent<Draggable>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Stops the grocery from rotating and falling over once it has initially landed from its spawnpoint
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Pauses the conveyor belt's movement on the item while it is being dragged
        if (draggable != null && draggable.IsDragging)
        {
            return;
        }
        // Moves the item along the conveyor belt if it is touching the conveyor belt
        if (isOnConveyor)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
    }
    // Handles collisions between objects
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Detects if the grocery has collided with the conveyor belt
        if (collision.gameObject.name == "ConveyorBelt")
        {
            isOnConveyor = true;
        }
        // Allows a grocery to rotate if it is colliding with another grocery
        else if (collision.gameObject.GetComponent<ConveyorItem>() != null)
        {
            ConveyorItem otherItem = collision.gameObject.GetComponent<ConveyorItem>();
            touchingItems.Add(otherItem); // Detects the groceries it is touching
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }
    // While the grocery is still in a collision
    void OnCollisionStay2D(Collision2D collision)
    {
        // If the conveyor belt is one of the objects it is touching, continue conveyor belt movement
        if (collision.gameObject.name == "ConveyorBelt")
        {
            isOnConveyor = true;
        }
    }
    // Handles when a grocery has stopped colliding
    void OnCollisionExit2D(Collision2D collision)
    {
        // If the grocery is no longer touching the conveyor belt
        if (collision.gameObject.name == "ConveyorBelt")
        {
            isOnConveyor = false;
            // Pause any passive horizontal movement unless the grocery is being dragged
            if (draggable == null || !draggable.IsDragging)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
        ConveyorItem otherItem = collision.gameObject.GetComponent<ConveyorItem>();
        // If the grocery is no longer touching other groceries, remove the touched groceries from the list
        if (otherItem != null)
        {
            touchingItems.Remove(otherItem);
        }
    }
}
