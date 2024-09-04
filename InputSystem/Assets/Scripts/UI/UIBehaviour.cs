using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBehaviour : MonoBehaviour
{
    public TextMeshProUGUI deviceNameDisplayText;
    public Image deviceDisplayIcon;

    public void UpdatePlayerDeviceNameDisplayText(string newDeviceName)
    {
        deviceNameDisplayText.SetText(newDeviceName);
    }

    public void UpdatePlayerIconDisplayColor(Color newColor)
    {
        deviceDisplayIcon.color = newColor;
    }
}