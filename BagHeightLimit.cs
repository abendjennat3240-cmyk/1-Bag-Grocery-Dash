// Controls the yellow dotted line's functions and fails the levels accordingly
using UnityEngine;
using System.Collections.Generic;

public class BagHeightLimit : MonoBehaviour
{
    private LevelManager levelManager;
    private Dictionary<ConveyorItem, float> settleTimers = new Dictionary<ConveyorItem, float>(); // Tracks how long a grocery is touching the yellow dotted line
    private float settleTime = 2f; // Amount of time that a stationary grocery must be touching the yellow dotted line in order to count the level as failed
    private bool levelFailed = false; // Stops the level from being failed more than one time 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }
    // When an item enters the yellow dotted line
    private void OnTriggerEnter2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>();
        // If the item is not a grocery, return
        if (item == null)
        {
            return;
        }
        // If the item is not already being tracked, track it
        if (!settleTimers.ContainsKey(item))
        {
            settleTimers[item] = 0f;
        }
    }
    // When an item leaves the yellow dotted line
    private void OnTriggerExit2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>();
        // If the item is a grocery, stop tracking it
        if (item != null)
        {
            settleTimers.Remove(item);
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Ensures the level has failed only once
        if (levelFailed)
        {
            return;
        }
        List<ConveyorItem> items = new List<ConveyorItem>(settleTimers.Keys);
        // For every item in the list
        foreach (ConveyorItem item in items)
        {
            // If the item does not exist, remove it
            if (item == null)
            {
                settleTimers.Remove(item);
                continue;
            }
            Draggable draggable = item.GetComponent<Draggable>();
            // If the item is being dragged, reset the timer
            if (draggable.IsDragging)
            {
                settleTimers[item] = 0f;
                continue;
            }
            settleTimers[item] += Time.deltaTime; // Increases the timer passively while the item is touching the yellow dotted line
            // If the item has been touching the yellow dotted line for long enough, the level is failed
            if (settleTimers[item] >= settleTime)
            {
                levelFailed = true;
                levelManager.FailLevel();
                return;
            }
        }
    }
    // Resets the height of the yellow dotted line for each level since it is lower on levels 6, 8, and 10
    public void ResetHeightLimit()
    {
        levelFailed = false;
        settleTimers.Clear();
    }
}
