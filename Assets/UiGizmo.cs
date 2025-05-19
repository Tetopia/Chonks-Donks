using UnityEngine;

public class UiGizmo : MonoBehaviour
{
    public Material materialNeutral;
    public Material materialHovered;
    public Material materialDragged;

    public MeshRenderer[] meshesWithMaterialToUpdate;
    public SkinnedMeshRenderer[] skinnedMeshesWithMaterialToUpdate;

    public VoidDelegateVoid onStartDrag, onEndDrag;

    protected void SetMaterial(Material mat)
    {
        foreach (MeshRenderer mr in meshesWithMaterialToUpdate) mr.material = mat;
        foreach (SkinnedMeshRenderer mr in skinnedMeshesWithMaterialToUpdate) mr.material = mat;
    }
}
