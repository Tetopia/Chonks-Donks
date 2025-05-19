using UnityEngine;

public class UiTranslator : UiGizmo
{
    Camera mainCamera;
    Vector3 lastMousePosition;
    bool isDragging = false;
    bool isHovered = false;

    public float moveSpeed = 0.3f;
    public float moveLimitLower = -0.5f;
    public float moveLimitUpper = 0.5f;
    float totalOffset;


    Vector3 localStartPosition;

    float value; //How far the slider is translated (normalized to -1 to 1)
    public VoidDelegateFloat OnValueChanged;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        localStartPosition = transform.localPosition;
        SetMaterial(materialNeutral);
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        ChooseMaterial();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        ChooseMaterial();
    }

    void OnMouseDown()
    {
        lastMousePosition = Input.mousePosition;
        isDragging = true;
        ChooseMaterial();
    }

    void OnMouseUp()
    {
        if(isDragging && onEndDrag != null) onEndDrag();
        isDragging = false;
        ChooseMaterial();
    }

    void Update()
    {
        if (isHovered && Input.GetMouseButtonDown(1))
        {
            transform.localPosition = localStartPosition;
            totalOffset = 0;
            OnValueChanged(0);
            if (onStartDrag != null) onStartDrag();
            return;
        }

        if (!isDragging) return;

        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        Vector3 camRight = mainCamera.transform.right;
        Vector3 camUp = mainCamera.transform.up;
        Vector3 worldMouseDelta = (camRight * mouseDelta.x + camUp * mouseDelta.y) * 0.01f;

        Vector3 worldUpAxis = transform.up; // object's local up in world space

        // Project world mouse movement onto the object's world-space up axis
        Vector3 movementWorld = Vector3.Project(worldMouseDelta, worldUpAxis);

        // Convert movement to local space relative to parent
        Vector3 movementLocal = transform.parent.InverseTransformVector(movementWorld);

        // Get local offset along the local up axis
        float deltaOffset = Vector3.Dot(movementLocal, Vector3.up) * moveSpeed;

        // Calculate new total offset and clamp it
        float newTotalOffset = Mathf.Clamp(totalOffset + deltaOffset, moveLimitLower, moveLimitUpper);

        // Determine actual delta (in case we hit limit)
        float clampedDelta = newTotalOffset - totalOffset;
        totalOffset = newTotalOffset;

        // Apply clamped movement in local space
        transform.localPosition += Vector3.up * clampedDelta;


        float normalizedOffset;
        if (totalOffset >= 0) normalizedOffset = totalOffset / moveLimitUpper;
        else normalizedOffset = totalOffset / -moveLimitLower;
        if (value != normalizedOffset)
        {
            value = normalizedOffset;
            if (OnValueChanged != null) OnValueChanged(value);
        }
    }

    void ChooseMaterial()
    {
        if (isDragging) SetMaterial(materialDragged);
        else if (isHovered) SetMaterial(materialHovered);
        else SetMaterial(materialNeutral);

        if (GetComponent<Animator>() != null)
        {
            if (isHovered && !isDragging) GetComponent<Animator>().Play("Play");
            else GetComponent<Animator>().Play("Stop");
        }
    }
}
