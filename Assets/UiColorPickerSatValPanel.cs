using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiColorPickerSatValPanel : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    bool initialized = false;
    RawImage satValImage;
    public Image satValCursor;
    RectTransform rectTransform, satValCursorTransform;
    UiColorPicker colorPicker;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        colorPicker = transform.parent.GetComponentInParent<UiColorPicker>();
        satValImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        satValCursorTransform = satValCursor.GetComponent<RectTransform>();
        satValCursorTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));
        initialized = true;
    }

    void UpdateColor(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float deltaX = rectTransform.sizeDelta.x * 0.5f;
            float deltaY = rectTransform.sizeDelta.y * 0.5f;

            // Clamp point to not be outside the rectTransform
            localPoint.x = Mathf.Clamp(localPoint.x, -deltaX, deltaX);
            localPoint.y = Mathf.Clamp(localPoint.y, -deltaY, deltaY);

            satValCursorTransform.localPosition = localPoint;

            float xNorm = (localPoint.x + deltaX) / rectTransform.sizeDelta.x;
            float yNorm = (localPoint.y + deltaY) / rectTransform.sizeDelta.y;

            satValCursor.color = Color.HSVToRGB(0, 0, 1 - yNorm);
            if(colorPicker!= null) colorPicker.SetSV(xNorm, yNorm);
        }
    }

    public void UpdateCursor(Color c)
    {
        if (!initialized) Init();

        float hue, sat, val;
        Color.RGBToHSV(c, out hue, out sat, out val);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        float localX = sat * rectTransform.sizeDelta.x - deltaX;
        float localY = val * rectTransform.sizeDelta.y - deltaY;

        satValCursorTransform.localPosition = new Vector2(localX, localY);
    }

    public void UpdateCursor(float x, float y)
    {
        if (!initialized) Init();

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        float localX = x * rectTransform.sizeDelta.x - deltaX;
        float localY = y * rectTransform.sizeDelta.y - deltaY;

        satValCursorTransform.localPosition = new Vector2(localX, localY);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }
}
