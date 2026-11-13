// Controls the sound effects for UI buttons
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // Plays the button hover audio when the player hovers their mouse over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayHover(); // Button hover audio
    }
    // Plays the button click audio when the player clicks the button with their mouse
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayClick(); // Button click audio
    }
}
