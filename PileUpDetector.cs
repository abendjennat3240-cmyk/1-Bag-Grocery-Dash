// Controls the detection of groceries on the conveyor belt piling up against the left side of the bag
using UnityEngine;
using System.Collections.Generic;

public class PileUpDetector : MonoBehaviour
{
    private LevelManager levelManager;
    private HashSet<ConveyorItem> itemsAtBag = new HashSet<ConveyorItem>(); // Groceries currently piled up at the end of the conveyor belt on the left side of the bag
    private float countdown = 5f; // Amount of time that groceries can be piled up before the level is failed
    private bool pileUpActive = false; // Whether a pileup is occurring
    private bool countdownSoundPlaying = false; // Whether the countdown audio is playing

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }
    // When an item is touching the left side of the bag
    void OnTriggerEnter2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>();
        // If the item is a grocery, add it to the list
        if (item != null)
        {
            itemsAtBag.Add(item);
        }
    }
    // When an item is no longer touching the left side of the bag
    void OnTriggerExit2D(Collider2D other)
    {
        ConveyorItem item = other.GetComponent<ConveyorItem>();
        // If the item is a grocery, remove it from the list
        if (item != null)
        {
            itemsAtBag.Remove(item);
        }
    }
    // Update is called once per frame
    void Update()
    {
        pileUpActive = false;
        // For every grocery touching the left side of the bag
        foreach (ConveyorItem item in itemsAtBag)
        {
            // If a grocery touching the left side of the bag is touching another grocery, detect a pileup
            if (item != null && item.IsTouchingAnotherItem)
            {
                pileUpActive = true;
                break;
            }
        }
        // While a pileup is active, activate the 5 second countdown
        if (pileUpActive)
        {
            // If the countdown audio is not yet playing, play it
            if (!countdownSoundPlaying)
            {
                AudioManager.Instance.StartCountdown(); // Countdown audio
                countdownSoundPlaying = true;
            }
            countdown -= Time.deltaTime; // Decreases the timer naturally
            levelManager.ShowPileUpCountdown(Mathf.CeilToInt(countdown)); // Displays the remaining time in seconds
            // If the countdown has reached 0, stop the countdown audio and mark the level as failed
            if (countdown <= 0f)
            {
                AudioManager.Instance.StopCountdown(); // Stops the countdown audio
                countdownSoundPlaying = false;
                levelManager.FailLevel();
            }
        }
        // If the countdown is still going
        else 
        {
            // If the countdown audio is playing, stop it
            if (countdownSoundPlaying)
            {
                AudioManager.Instance.StopCountdown(); // Stops the countdown audio
                countdownSoundPlaying = false;
            }
            countdown = 5f; // Resets the countdown to 5 seconds for the next trigger
            levelManager.ShowLevelNumber(); // Displays the current level on the checkout counter again
        }
    }
    // Clears all currently piled up groceries
    public void ResetPileUp()
    {
        itemsAtBag.Clear();
        countdown = 5f; // Resets the countdown to 5 seconds
        pileUpActive = false;
    }
}
