using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameProgressManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int maxScore = 100;
    [SerializeField] private int pointsPerOrder = 10;
    [SerializeField] private int pointsLostOnFail = 5;

    [Header("Star Thresholds")]
    [SerializeField] private int firstStarThreshold = 25;
    [SerializeField] private int secondStarThreshold = 60;
    [SerializeField] private int thirdStarThreshold = 100;

    [Header("UI References")]
    [SerializeField] private Image scoreFillImage;
    [SerializeField] private RectTransform scoreFillTransform; // the full bar container
    [SerializeField] private RectTransform firstStar;
    [SerializeField] private RectTransform secondStar;
    [SerializeField] private RectTransform thirdStar;

    private int currentScore = 0;
    private Coroutine fillAnimationCoroutine;

    private void Start()
    {
        PlaceStars();
        UpdateUIInstant(); // Initialize fill instantly
    }

    private void PlaceStars()
    {
        float barHeight = scoreFillTransform.rect.height;

        PositionStar(firstStar, firstStarThreshold, barHeight);
        PositionStar(secondStar, secondStarThreshold, barHeight);
        PositionStar(thirdStar, thirdStarThreshold, barHeight);
    }

    private void PositionStar(RectTransform star, int threshold, float barHeight)
    {
        float normalizedY = (float)threshold / maxScore;
        star.anchoredPosition = new Vector2(star.anchoredPosition.x, normalizedY * barHeight);
        star.gameObject.SetActive(true); // always visible
    }

    public void GainPoints()
    {
        int oldScore = currentScore;
        currentScore += pointsPerOrder;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        AnimateFill(oldScore, currentScore);
    }

    public void LosePoints()
    {
        int oldScore = currentScore;
        currentScore -= pointsLostOnFail;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        AnimateFill(oldScore, currentScore);
    }

    private void AnimateFill(int fromScore, int toScore)
    {
        float fromFill = (float)fromScore / maxScore;
        float toFill = (float)toScore / maxScore;

        if (fillAnimationCoroutine != null)
            StopCoroutine(fillAnimationCoroutine);

        fillAnimationCoroutine = StartCoroutine(AnimateFillRoutine(fromFill, toFill, 0.5f)); // animate over 0.5 seconds
    }

    private IEnumerator AnimateFillRoutine(float fromFill, float toFill, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            scoreFillImage.fillAmount = Mathf.Lerp(fromFill, toFill, t);
            yield return null;
        }
        scoreFillImage.fillAmount = toFill;
    }

    private void UpdateUIInstant()
    {
        float fillAmount = (float)currentScore / maxScore;
        scoreFillImage.fillAmount = fillAmount;
    }

    public void ResetProgress()
    {
        int oldScore = currentScore;
        currentScore = 0;
        AnimateFill(oldScore, currentScore);
    }
}
