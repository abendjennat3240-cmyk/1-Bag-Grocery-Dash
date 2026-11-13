// Controls the game audio effects
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource conveyorSource;
    public AudioSource countdownSource;
    public AudioSource effectsSource;
    public AudioClip conveyorLoopClip;
    public AudioClip countdownClip;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;
    public AudioClip groceryPickupClip;
    public AudioClip groceryBaggedClip;
    public AudioClip levelCompleteClip;
    public AudioClip levelFailedClip;
    public AudioClip gameVictoryClip;

    // Called when the script is being loaded
    void Awake()
    {
        Instance = this;
    } 
    // Starts the conveyor belt audio
    public void StartConveyor()
    {
        // Assigns and plays the conveyor belt audio if it is not already playing
        if (!conveyorSource.isPlaying)
        {
            conveyorSource.clip = conveyorLoopClip;
            conveyorSource.loop = true; // Loops the audio
            conveyorSource.Play();
        }
    }
    // Stops the conveyor belt audio
    public void StopConveyor()
    {
        conveyorSource.Stop();
    }
    // Starts the countdown audio for when groceries pile up on the conveyor belt
    public void StartCountdown()
    {
        countdownSource.Stop(); // Stops any previously playing countdown audio
        countdownSource.clip = countdownClip;
        countdownSource.loop = false; // No loop for the countdown audio
        countdownSource.Play();
    }
    // Stops the countdown audio
    public void StopCountdown()
    {
        countdownSource.Stop();
    }
    // Plays the button hover audio
    public void PlayHover()
    {
        effectsSource.PlayOneShot(buttonHoverClip);
    }
    // Plays the button click sound
    public void PlayClick()
    {
        effectsSource.PlayOneShot(buttonClickClip);
    }
    // Plays the grocery picked up sound
    public void PlayPickup()
    {
        effectsSource.PlayOneShot(groceryPickupClip);
    }
    // Plays the grocery successfully bagged sound
    public void PlayBagged()
    {
        effectsSource.PlayOneShot(groceryBaggedClip);
    }
    // Plays the level complete sound
    public void PlayLevelComplete()
    {
        effectsSource.PlayOneShot(levelCompleteClip);
    }
    // Plays the level failed sound
    public void PlayLevelFailed()
    {
        effectsSource.PlayOneShot(levelFailedClip);
    }
    // Plays the game victory sound at the very end of the game
    public void PlayVictory()
    {
        effectsSource.PlayOneShot(gameVictoryClip);
    }
}
