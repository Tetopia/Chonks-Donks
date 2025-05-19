using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UiColorPicker : MonoBehaviour
{
    bool initialized = false;
    float currentHue, currentSat, currentVal = 0.5f;

    public TMP_Text title;
    public RawImage hueImage, satValImage;
    public Image outputOrigImage, outputNewImage;
    public Slider hueSlider;
    public TMP_InputField hexInputField;
    public VoidDelegateVoid onConfirm, onColorChange, onClose;
    public Color startColor;

    Texture2D hueTexture, svTexture;

    UiColorPickerSatValPanel svPanel;

    private void Start()
    {
        if (!initialized) Init();
    }


    private void OnDisable()
    {
        if (onClose != null) onClose();
    }

    public void Open(Color color, VoidDelegateVoid OnConfirm, VoidDelegateVoid OnColorChange = null, VoidDelegateVoid OnClose = null)
    {
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);
        onConfirm = OnConfirm;
        onColorChange = OnColorChange;
        onClose = OnClose;
        Init();
        gameObject.SetActive(true);
        SetColor(color);
        startColor = color;
    }

    void Init()
    {
        svPanel = satValImage.GetComponent<UiColorPickerSatValPanel>();
        CreateHueImage();
        CreateSatValImage();
        UpdateOutput();
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
        outputOrigImage.color = currentColor;
        outputNewImage.color = currentColor;
        initialized = true;
    }

    void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";
        
        for(int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB(((float)i) / hueTexture.height, 1, 1));
        }
        hueTexture.Apply();
        hueImage.texture = hueTexture;
    }

    void CreateSatValImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, ((float)x) / svTexture.width, ((float)y) / svTexture.height));
            }
        }
        svTexture.Apply();
        satValImage.texture = svTexture;
    }

    void UpdateOutput()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
        outputNewImage.color = currentColor;
        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColor);
        if (onColorChange != null) onColorChange();
    }

    public void SetSV(float s, float v)
    {
        currentSat = s;
        currentVal = v;

        UpdateOutput();
    }

    public void SetColor(Color newCol)
    {
        Color.RGBToHSV(newCol, out currentHue, out currentSat, out currentVal);
        
        hueSlider.value = currentHue;
        hexInputField.text = "";
        UpdateOutput();
        svPanel.UpdateCursor(newCol);
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();
        UpdateOutput();
    }

    public void OnTextInput()
    {
        if (hexInputField.text.Length < 6) return;
        Color newCol;
        if (ColorUtility.TryParseHtmlString("#" + hexInputField.text, out newCol)) SetColor(newCol);
    }

    public Color GetSelectedColor()
    {
        return Color.HSVToRGB(currentHue, currentSat, currentVal);
    }

    public void Confirm()
    {
        gameObject.SetActive(false);
        if (onConfirm != null) onConfirm();
    }
}
