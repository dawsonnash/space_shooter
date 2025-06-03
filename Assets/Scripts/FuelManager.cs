using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    public Image fuelFillImage;             // Assign your Fill image here
    public float maxFuel = 100f;
    public float fuelDrainRate = 20f;       // Fuel drained per second when boosting
    public float fuelRechargeRate = 10f;    // Fuel regained per second when not boosting

    private float currentFuel;

    public PlayerMovement player;           // Reference to your PlayerMovement script

    void Start()
    {
        currentFuel = maxFuel;
    }

    void Update()
    {
        if (player != null && player.IsBoosting() && currentFuel > 0f)
        {
            currentFuel -= fuelDrainRate * Time.deltaTime;
        }

        // Do NOT regenerate fuel while idle
        // (Removed the recharge code)

        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        if (fuelFillImage != null)
        {
            float percent = currentFuel / maxFuel;
            fuelFillImage.fillAmount = percent;

            // Color shift: Green → Yellow → Red
            if (percent > 0.5f)
            {
                // Green to Yellow
                fuelFillImage.color = Color.Lerp(Color.yellow, Color.green, (percent - 0.5f) * 2);
            }
            else
            {
                // Yellow to Red
                fuelFillImage.color = Color.Lerp(Color.red, Color.yellow, percent * 2);
            }
        }

    }


    public bool HasFuel()
    {
        return currentFuel > 0f;
    }
}
