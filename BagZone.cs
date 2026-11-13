// Controls how groceries being successfully bagged works
using UnityEngine;
using System.Collections.Generic;

public class BagZone : MonoBehaviour
{
    public int groceriesInBag = 0; // Amount of groceries that are successfully bagged
    private Dictionary<ConveyorItem, float> settleTimers = new Dictionary<ConveyorItem, float>(); // Times how long the grocery has been settled within the bagging zone
    private HashSet<ConveyorItem> settledGroceries = new HashSet<ConveyorItem>(); // Successfully bagged groceries
    private float settleTime = 0.5f; // Amount of time that a grocery must be settled for before being counted as successfully bagged

    // While an item is within the bag zone
    private void OnTriggerStay2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>(); // Checks whether the item is a grocery
        // Ignores any items that are not groceries or were already counted
        if (item == null || settledGroceries.Contains(item))
        {
            return;
        }
        Draggable draggable = item.GetComponent<Draggable>();
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        // If the item is being dragged, reset the timer so it cannot be marked as bagged yet
        if (draggable.IsDragging)
        {
            settleTimers[item] = 0f;
            return;
        }
        Bounds bagBounds = GetComponent<Collider2D>().bounds;
        Bounds itemBounds = other.bounds;
        bool fullyInside = bagBounds.Contains(itemBounds.min) && bagBounds.Contains(itemBounds.max); // Whether the grocery is fully inside the bagging zone
        // If the item is not fully inside the bag, reset the timer so it cannot be marked as bagged yet
        if (!fullyInside)
        {
            settleTimers[item] = 0f;
            return;
        }
        // If the item is still moving, reset the timer so it cannot be marked as bagged
        if (rb.linearVelocity.magnitude > 10.0f || Mathf.Abs(rb.angularVelocity) > 25.0f)
        {
            settleTimers[item] = 0f;
            return;
        }
        // Initializes the item's timer to 0
        if (!settleTimers.ContainsKey(item))
        {
            settleTimers[item] = 0f;
        }
        settleTimers[item] += Time.fixedDeltaTime; // Increases the item's timer while all conditions are met
        // Marks the grocery as bagged if it has met all conditions for long enough time
        if (settleTimers[item] >= settleTime)
        {
            settledGroceries.Add(item);
            groceriesInBag++;
            AudioManager.Instance.PlayBagged(); // Successfully bagged audio
        }
    }
    // When an item has left the bagging zone
    private void OnTriggerExit2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>(); // Checks whether the item is a grocery
        // If the item is a grocery
        if (item != null)
        {
            settleTimers.Remove(item); // Removes the item's timer
            // If the item is already counted as bagged, remove it from the total
            if (settledGroceries.Remove(item))
            {
                groceriesInBag--;
            }
        }
    }
    // Resets the statistics for the next level
    public void ResetBag()
    {
        groceriesInBag = 0;
        settleTimers.Clear();
        settledGroceries.Clear();
    }
}