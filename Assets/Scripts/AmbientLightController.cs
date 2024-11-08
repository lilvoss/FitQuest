using UnityEngine;
using UnityEngine.UI;

public class AmbientLightController : MonoBehaviour
{
    public Text statusText; // Assign this in the Inspector
    private float currentLightLevel;

    void Start()
    {
        // Start checking ambient light levels
        InvokeRepeating("CheckAmbientLight", 0f, 5f); // Check every 5 seconds
    }

    void CheckAmbientLight()
    {
        // Simulating light sensor reading. You would replace this with actual sensor data.
        currentLightLevel = GetAmbientLightLevel();

        // Update UI or screen brightness based on light level
        UpdateUI(currentLightLevel);
    }

    private float GetAmbientLightLevel()
    {
        // Replace this with actual ambient light sensor data
        // For demonstration purposes, we are using a random value
        return Random.Range(0f, 100f); // Simulating light level from 0 to 100
    }

    private void UpdateUI(float lightLevel)
    {
        // Example logic to change brightness or UI theme
        if (lightLevel < 30f)
        {
            // Low light - switch to dark mode
            Camera.main.backgroundColor = Color.black; // Change background color
            statusText.text = "Ambient Light Level: Low";
        }
        else if (lightLevel >= 30f && lightLevel < 70f)
        {
            // Medium light - adjust brightness
            Camera.main.backgroundColor = Color.gray;
            statusText.text = "Ambient Light Level: Medium";
        }
        else
        {
            // High light - switch to light mode
            Camera.main.backgroundColor = Color.white;
            statusText.text = "Ambient Light Level: High";
        }
    }
}
