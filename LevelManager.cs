// Controls the level progression and display as well as general UI panels
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public GameObject introPanel;
    public GameObject levelCompletePanel;
    public GameObject levelFailedPanel;
    public GameObject victoryPanel;
    private int currentLevel = 1;
    private GrocerySpawner grocerySpawner;
    private BagZone bagZone;
    private bool levelEnded = false;
    public TMP_Text levelDisplay; // Text on the checkout counter under the conveyor belt
    public float originalLevelDisplayX; // Original x position of the text on the checkout counter under the conveyor belt as the countdown was moved slightly for aesthetic purposes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.StopConveyor(); // Pauses the conveyor audio when the game first launches
        grocerySpawner = FindAnyObjectByType<GrocerySpawner>();
        bagZone = FindAnyObjectByType<BagZone>();
        Time.timeScale = 0f; // The game is paused until the player begins
        introPanel.SetActive(true); // Turns on the intro screen
        levelCompletePanel.SetActive(false);
        levelFailedPanel.SetActive(false);
        victoryPanel.SetActive(false);
        UpdateLevelDisplay(); // Displays the first level number
        originalLevelDisplayX = levelDisplay.rectTransform.anchoredPosition.x; // Notes the x position of the original level display so it can be reset after a countdown that moves it occurs
    }
    // Update is called once per frame
    void Update()
    {
        // If all groceries in the level have spawned and been successfully bagged, the level is completed
        if (!levelEnded && grocerySpawner.HasFinishedSpawning && bagZone.groceriesInBag == grocerySpawner.TotalItemsInLevel)
        {
            levelEnded = true;
            CompleteLevel();
        }
    }
    // Starts the game from Level 1
    public void PlayGame()
    {
        introPanel.SetActive(false); // Disables the intro screen to begin the game
        Time.timeScale = 1f;
        currentLevel = 1;
        levelEnded = false;
        UpdateLevelDisplay();
        grocerySpawner.StartLevel(1); // Starts spawning groceries for Level 1
        AudioManager.Instance.StartConveyor(); // Conveyor belt audio
    }
    // Completes the level when all groceries have been successfully bagged
    public void CompleteLevel()
    {
        AudioManager.Instance.StopConveyor(); // Stops the conveyor belt audio
        // If the level completed was Level 10, show the victory screen
        if (currentLevel == 10)
        {
            victoryPanel.SetActive(true);
            AudioManager.Instance.PlayVictory(); // Victory audio
        }
        // Otherwise, show the level complete screen
        else
        {
            levelCompletePanel.SetActive(true);
            AudioManager.Instance.PlayLevelComplete(); // Level complete audio
        }
        Time.timeScale = 0f; // The game is paused while completion panels are displayed onscreen
    }
    // Continues to the next level
    public void ContinueToNextLevel()
    {
        levelCompletePanel.SetActive(false); // Hides the level complete screen
        Time.timeScale = 1f;
        currentLevel++; // Increases the current level by 1
        UpdateLevelDisplay(); // Updates the level display on the checkout counter
        levelEnded = false;
        grocerySpawner.StartLevel(currentLevel); // Starts spawning groceries for the next level
        AudioManager.Instance.StartConveyor(); // Conveyor belt audio
    }
    // The player failed the level
    public void FailLevel()
    {
        // Ensures the failure only occurs once
        if (levelEnded)
        {
            return;
        }
        levelEnded = true;
        AudioManager.Instance.StopConveyor(); // Stops the conveyor belt audio
        AudioManager.Instance.StopCountdown(); // Stops any countdown audio
        AudioManager.Instance.PlayLevelFailed(); // Level failed audio
        levelFailedPanel.SetActive(true); // Displays the level failed screen
        Time.timeScale = 0f; // Pauses the game while the screen is displayed
    }
    // Restarts the level after it is failed
    public void RetryLevel()
    {
        levelFailedPanel.SetActive(false); // Hides the level failed screen
        Time.timeScale = 1f;
        levelEnded = false;
        grocerySpawner.StartLevel(currentLevel); // Starts the level's grocery queue again
        AudioManager.Instance.StartConveyor(); // Conveyor belt audio
    }
    // Play again from Level 1 after all levels are completed
    public void PlayAgain()
    {
        victoryPanel.SetActive(false); // Disables the victory screen
        Time.timeScale = 1f;
        currentLevel = 1; // Resets the current level to Level 1
        levelEnded = false;
        UpdateLevelDisplay(); // Updates the level display on the checkout counter to Level 1
        grocerySpawner.StartLevel(1);
        AudioManager.Instance.StartConveyor(); // Conveyor belt audio
    }
    // Changes the text on the checkout counter to display the current level
    void UpdateLevelDisplay()
    {
        levelDisplay.text = "Level " + currentLevel;
    }
    // Changes the text on the checkout counter to display the pileup countdown
    public void ShowPileUpCountdown(int seconds)
    {
        Vector2 position = levelDisplay.rectTransform.anchoredPosition; 
        position.x = 0f; // Moves the x position of the text for the countdown to be centered on the checkout counter
        levelDisplay.rectTransform.anchoredPosition = position;
        levelDisplay.text = " " + seconds; // Displays the countdown text
    }
    // Reverts the current level display on the checkout counter after the countdown ends
    public void ShowLevelNumber()
    {
        Vector2 position = levelDisplay.rectTransform.anchoredPosition;
        position.x = originalLevelDisplayX; // Reverts the position of the text on the checkout counter
        levelDisplay.rectTransform.anchoredPosition = position;
        UpdateLevelDisplay(); // Displays the current level on the checkout counter
    }
}
