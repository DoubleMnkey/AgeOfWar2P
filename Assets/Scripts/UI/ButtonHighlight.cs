using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        Color color = image.color;

        color.a =
            Mathf.Lerp(
                75f / 255f,
                150f / 255f,
                (Mathf.Sin(Time.time * 5f) + 1f) / 2f
            );

        image.color = color;
    }
}