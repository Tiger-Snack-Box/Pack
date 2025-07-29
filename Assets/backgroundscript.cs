using UnityEngine;

public class backgroundscript : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / (float)Screen.height;

        Vector2 spriteSize = sr.sprite.bounds.size;

        // Calculate scale needed to fill screen (maintaining aspect ratio)
        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        // Use the larger scale so the image fills the screen and is cropped if needed
        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
