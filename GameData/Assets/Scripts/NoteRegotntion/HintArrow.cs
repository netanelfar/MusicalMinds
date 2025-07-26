using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Animated hint arrow that slides and points left/right based on note distance from target
public class HintArrow : MonoBehaviour
{
    [Header("Arrow Direction")]
    public bool pointLeft = false;

    [Header("Animation Settings")]
    public float slideDistance = 80f;         
    public float slideSpeed = 120f;        
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 0.7f, 1); 
    
    private RectTransform rectTransform;
    private Vector2 startPosition;
    public Image arrowImage;

    // Get RectTransform component reference.
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Store initial position and set arrow direction.
    void Start()
    {
        startPosition = rectTransform.anchoredPosition;
        UpdateDirection();
    }

    // Flips arrow to point left.
    public void PointLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }

    // Resets arrow to point right.
    public void PointRight()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    // Sets point direction and immediately updates visual.
    public void SetPointLeft(bool shouldPointLeft)
    {
        pointLeft = shouldPointLeft;
        UpdateDirection();
    }

    // Updates arrow direction.
    private void UpdateDirection()
    {
        if (pointLeft)
            PointLeft();
        else
            PointRight();
    }

    // Animates slide with distance based on how far off the note guess was.
    public void AnimateSlide(int noteDistance)
    {
        float actualSlideDistance;
        int absDistance = Mathf.Abs(noteDistance); // White keys off.

        if (absDistance <= 2)
        {
            actualSlideDistance = slideDistance * 0.6f;  // Small nudge
        }
        else if (absDistance <= 5)
        {
            actualSlideDistance = slideDistance * 1.0f;  // Normal
        }
        else
        {
            actualSlideDistance = slideDistance * 1.4f;  // Big miss
        }
        StartCoroutine(SlideAnimation(actualSlideDistance));
    }

    // Slides arrow out.
    private IEnumerator SlideAnimation(float distance)
    {
        // Determine slide direction.
        Vector2 slideDirection = pointLeft ? Vector2.left : Vector2.right;
        Vector2 targetPosition = startPosition + (slideDirection * distance);

        float elapsedTime = 0f;
        float slideTime = distance / slideSpeed;

        // Slide out.
        while (elapsedTime < slideTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slideTime;
            float curveValue = slideCurve.Evaluate(progress);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // slide time.
        gameObject.SetActive(false);
    }

    // Simple version without calculating note distance!!!!!!!!!!!!!!!
    public void AnimateSlideSimple()
    {
        StartCoroutine(SlideAnimation(slideDistance));
    }

    // Changes arrow color.
    public void SetArrowColor(Color color)
    {
        if (arrowImage != null)
            arrowImage.color = color;
    }

    // Displays a directional arrow hint.
    public void ShowHint(int noteDistance)
    {
        bool shouldGoLeft = noteDistance < 0;
        int absDistance = Mathf.Abs(noteDistance); 
        SetPointLeft(shouldGoLeft);

        // Set color based on distance
        if (absDistance <= 3)
            SetArrowColor(Color.green);      
        else if (absDistance <= 7)
            SetArrowColor(new Color(1f, 0.64f, 0f)); 
        else
            SetArrowColor(Color.red);        

        AnimateSlide(absDistance); 
        Destroy(gameObject, 2.5f);
    }

}