using UnityEngine;
using UnityEngine.UI;

public class PaintWithMouse : MonoBehaviour
{
    //Source Tutorial: https://medium.com/@unitydevmaster/paint-the-surfaces-of-3d-objects-using-shader-graph-d1f975567268

    public Camera cam;
    public Shader whitePatternShader, colorDrawShader;

    RenderTexture whitePatternMap, customColorTexture;
    Material currentMaterial, whitePatternMaterial, customColorMaterial;
    Color customColor;
    RaycastHit hit;
    Collider coll;

    public bool isLeftSide;
    [Range(0,1)]
    public float strength;
    [Range(1,10)]
    public float size;
    public bool invertInput;
    public bool drawOnPattern;

    public UiGizmo[] gizmosVisibleDuringPainting;

    bool ignoreInput = false;

    string textureSaveLocation = "Model/Textures/Generated/tex";

    void Start()
    {
        textureSaveLocation = Application.dataPath + "/" + textureSaveLocation + (isLeftSide ? "L.png" : "R.png");

        coll = GetComponent<Collider>();

        whitePatternMaterial = new Material(whitePatternShader);
        whitePatternMaterial.SetVector("_Color", Color.red);

        customColorMaterial = new Material(colorDrawShader);
        customColorMaterial.SetVector("_Color", Color.gray);

        currentMaterial = new Material(GetComponent<SkinnedMeshRenderer>().material);
        GetComponent<SkinnedMeshRenderer>().material = currentMaterial;

        whitePatternMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_WhitePatternMap", whitePatternMap);

        customColorTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        currentMaterial.SetTexture("_CustomColorTexture", customColorTexture);

        DrawPatternTexture();
        InitCustomColorTexture();
        //DrawCustomColorTexture();

        foreach (UiGizmo ug in gizmosVisibleDuringPainting)
        {
            ug.onStartDrag += () => { ignoreInput = true; };
            ug.onEndDrag += () => { ignoreInput = false; };
        }

        enabled = false;
    }

    private void OnEnable()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        Mesh bakedMesh = new Mesh();
        smr.BakeMesh(bakedMesh);
        MeshCollider col = GetComponent<MeshCollider>();
        col.sharedMesh = bakedMesh;
        //Needs to enable "Read/Write" Property of the mesh or it can't be used in a build!
    }

    void Update()
    {
        if (ignoreInput) return;

        bool leftMouse = Input.GetMouseButton(0);
        bool rightMouse = Input.GetMouseButton(1);
        if (leftMouse || rightMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (coll.Raycast(ray, out hit, 100.0f))
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("Paintable")) && hit.collider == coll)
            {
                if (drawOnPattern)
                {
                    whitePatternMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                    if ((leftMouse && !invertInput) || (rightMouse && invertInput)) whitePatternMaterial.SetFloat("_Strength", strength);
                    else whitePatternMaterial.SetFloat("_Strength", -strength);
                    whitePatternMaterial.SetFloat("_Size", size);
                    DrawPatternTexture();
                }
                else
                {
                    customColorMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                    customColorMaterial.SetFloat("_Strength", strength);
                    if ((leftMouse && !invertInput) || (rightMouse && invertInput)) customColorMaterial.SetVector("_Color", customColor);
                    else customColorMaterial.SetVector("_Color", new Color(0,0,0,0));
                    customColorMaterial.SetFloat("_Size", size);
                    DrawCustomColorTexture();
                }
            }
        }
    }

    void DrawPatternTexture()
    {
        RenderTexture temp = RenderTexture.GetTemporary(whitePatternMap.width, whitePatternMap.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(whitePatternMap, temp);
        Graphics.Blit(temp, whitePatternMap, whitePatternMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }

    void InitCustomColorTexture()
    {
        Texture2D emptyTex = new Texture2D(16, 16);
        for (int y = 0; y < customColorTexture.height; y++)
        {
            for (int x = 0; x < customColorTexture.width; x++)
            {
                emptyTex.SetPixel(x, y, new Color(0,0,0,0));
            }
        }
        emptyTex.Apply();
        RenderTexture.active = customColorTexture;
        Graphics.Blit(emptyTex, customColorTexture);
    }

    void DrawCustomColorTexture()
    {
        RenderTexture temp = RenderTexture.GetTemporary(customColorTexture.width, customColorTexture.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(customColorTexture, temp);
        Graphics.Blit(temp, customColorTexture, customColorMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }

    public void SetInvertInput(bool invert)
    {
        invertInput = invert;
    }

    internal void SetColor(Color color)
    {
        customColor = color;
    }

    public void SaveRenderTextureToFile()
    {
        Texture2D tex;
        tex = new Texture2D(customColorTexture.width, customColorTexture.height, TextureFormat.ARGB32, false, false);
        var oldRt = RenderTexture.active;
        RenderTexture.active = customColorTexture;
        tex.ReadPixels(new Rect(0, 0, customColorTexture.width, customColorTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = oldRt;
        System.IO.File.WriteAllBytes(textureSaveLocation, tex.EncodeToPNG());
        if (Application.isPlaying)
            Object.Destroy(tex);
        else
            Object.DestroyImmediate(tex);

    }
}
