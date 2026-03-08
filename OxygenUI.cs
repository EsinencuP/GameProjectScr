using UnityEngine;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    public PlayerOxygen player;
    public Image oxygenBar;

    void Update()
    {
        float current = player.GetOxygen();
        float max = player.GetMaxOxygen();

        oxygenBar.fillAmount = current / max;
    }
}