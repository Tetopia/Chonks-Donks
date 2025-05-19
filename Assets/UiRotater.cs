using UnityEngine;

public class UiRotater : UiGizmo
{
    bool isDragging = false;
    bool startDrag = false;
    Vector3 lastHitPoint;

    public Transform objectToRotate;
    public Transform torusCollider;
    public Transform rotationPlane;

    private void Start()
    {
        SetMaterial(materialNeutral);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hasHitTorus = Physics.Raycast(ray, out hit, LayerMask.GetMask("UI")) && hit.transform == torusCollider;
        bool hasHitPlane = Physics.Raycast(ray, out hit, LayerMask.GetMask("UI")) && hit.transform == rotationPlane;

        if (isDragging) SetMaterial(materialDragged);
        else if (hasHitTorus) SetMaterial(materialHovered);
        else SetMaterial(materialNeutral);

        if (Input.GetMouseButtonDown(0))
        {
            if (hasHitTorus)
            {
                isDragging = true;
                lastHitPoint = hit.point;
                torusCollider.GetComponent<Collider>().enabled = false;
                rotationPlane.gameObject.SetActive(true);
                startDrag = true;
                if (onStartDrag != null) onStartDrag();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            torusCollider.GetComponent<Collider>().enabled = true;
            rotationPlane.gameObject.SetActive(false);
            if (onEndDrag != null) onEndDrag();
        }

        if (isDragging && hasHitPlane)
        {
            Vector3 currentHitPoint = hit.point;
            Vector3 rotationAxis = Vector3.up;
            Vector3 center = transform.position;

            // Project points onto rotation plane
            Vector3 from = Vector3.ProjectOnPlane(lastHitPoint - center, rotationAxis);
            Vector3 to = Vector3.ProjectOnPlane(currentHitPoint - center, rotationAxis);

            if (from != Vector3.zero && to != Vector3.zero)
            {
                float angle = Vector3.SignedAngle(from, to, rotationAxis);
                if (!startDrag)
                {
                    transform.Rotate(rotationAxis, angle, Space.World);
                    objectToRotate.Rotate(rotationAxis, angle, Space.World);
                }
                lastHitPoint = currentHitPoint;
                startDrag = false;
            }
        }
    }
}
