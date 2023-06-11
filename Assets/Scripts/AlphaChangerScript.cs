using UnityEngine;
using UnityEngine.UI;

public class AlphaChangerScript : MonoBehaviour
{
    public float alphaValue = 1;
    public SystemController SystemController;
    public Image image;

    private void Update()
    {
        Color imageColor = image.color;
        imageColor.a = alphaValue;
        image.color = imageColor;
    }
}