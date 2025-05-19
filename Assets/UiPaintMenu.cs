using UnityEngine;
using UnityEngine.UI;

public class UiPaintMenu : MonoBehaviour
{
    public Image baseColorPreview, pointsColorPreview, pangareColorPreview, customColorPreview;
    public UiColorPicker colorPicker;
    public PaintWithMouse[] painters;
    public Toggle customColorToggle;
    public Image[] buttonImages;
    public Color highlightColor = Color.yellow;

    private void Start()
    {
        baseColorPreview.color = painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_BaseCol");
        pointsColorPreview.color = painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_PointsCol");
        pangareColorPreview.color = painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_PangareCol");
        customColorPreview.color = Color.gray;
        foreach (PaintWithMouse pwm in painters) pwm.SetColor(customColorPreview.color);
        buttonImages[4].color = highlightColor;
    }

    private void OnDisable()
    {
        StopBrush();
    }

    public void SetBrushSize(float size)
    {
        foreach (PaintWithMouse pwm in painters) pwm.size = size;
    }

    public void SetBrushStrength(float strength)
    {
        foreach (PaintWithMouse pwm in painters) pwm.strength = strength;
    }

    public void TogglePointsEnabled(bool enabled)
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetInt("_PointsEnabled", enabled ? 1 : 0);
    }

    public void TogglePangareEnabled(bool enabled)
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetInt("_PangareEnabled", enabled ? 1 : 0);
    }

    public void ToggleCustomColorEnabled(bool enabled)
    {
        foreach (PaintWithMouse pwm in painters)
        {
            pwm.drawOnPattern = !enabled;
            pwm.GetComponent<SkinnedMeshRenderer>().material.SetInt("_CustomColorEnabled", enabled ? 1 : 0);
        }
    }

    public void PangareAmount(float value)
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_PangareExpression", value);
    }

    public void PangareBlur(float value)
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_PangareBlur", value);
    }

    public void OpenColorPickerBase()
    {
        colorPicker.Open(painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_BaseCol"), OnConfirmHorseBaseColor, null, OnColorPickerClose);
        foreach (PaintWithMouse pwm in painters) pwm.enabled = false;
    }

    public void OpenColorPickerPoints()
    {
        colorPicker.Open(painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_PointsCol"), OnConfirmHorsePointsColor, null, OnColorPickerClose);
        foreach (PaintWithMouse pwm in painters) pwm.enabled = false;
    }

    public void OpenColorPickerCountershading()
    {
        colorPicker.Open(painters[0].GetComponent<SkinnedMeshRenderer>().material.GetColor("_PangareCol"), OnConfirmHorsePangareColor, null, OnColorPickerClose);
        foreach (PaintWithMouse pwm in painters) pwm.enabled = false;
    }

    public void OpenColorPickerCustomColor()
    {
        colorPicker.Open(customColorPreview.color, OnConfirmHorseCustomColor, null, OnColorPickerClose);
        foreach (PaintWithMouse pwm in painters) pwm.enabled = false;
    }

    void OnColorPickerClose()
    {
        foreach (PaintWithMouse pwm in painters) pwm.enabled = true;
    }

    void OnConfirmHorseBaseColor()
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetColor("_BaseCol", colorPicker.GetSelectedColor());
        baseColorPreview.color = colorPicker.GetSelectedColor();
    }

    void OnConfirmHorsePointsColor()
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetColor("_PointsCol", colorPicker.GetSelectedColor());
        pointsColorPreview.color = colorPicker.GetSelectedColor();
    }

    void OnConfirmHorsePangareColor()
    {
        foreach (PaintWithMouse pwm in painters) pwm.GetComponent<SkinnedMeshRenderer>().material.SetColor("_PangareCol", colorPicker.GetSelectedColor());
        pangareColorPreview.color = colorPicker.GetSelectedColor();
    }

    void OnConfirmHorseCustomColor()
    {
        foreach (PaintWithMouse pwm in painters) pwm.SetColor(colorPicker.GetSelectedColor());// pwm.GetComponent<SkinnedMeshRenderer>().material.SetColor("_PangareCol", colorPicker.GetSelectedColor());
        customColorPreview.color = colorPicker.GetSelectedColor();
    }

    public void UseWhiteMarkingPaint()
    {
        foreach (Image i in buttonImages) i.color = Color.white;
        buttonImages[0].color = highlightColor;
        foreach (PaintWithMouse pwm in painters)
        {
            pwm.enabled = true;
            pwm.SetInvertInput(false);
            pwm.drawOnPattern = true;
        }
    }

    public void UseWhiteMarkingEraser()
    {
        foreach (Image i in buttonImages) i.color = Color.white;
        buttonImages[1].color = highlightColor;
        foreach (PaintWithMouse pwm in painters)
        {
            pwm.enabled = true;
            pwm.SetInvertInput(true);
            pwm.drawOnPattern = true;
        }
    }

    public void UseCustomColorPaint()
    {
        foreach (Image i in buttonImages) i.color = Color.white;
        buttonImages[2].color = highlightColor;
        customColorToggle.isOn = true;
        foreach (PaintWithMouse pwm in painters)
        {
            pwm.enabled = true;
            pwm.SetInvertInput(false);
            pwm.drawOnPattern = false;
        }
    }

    public void UseCustomColorEraser()
    {
        foreach (Image i in buttonImages) i.color = Color.white;
        buttonImages[3].color = highlightColor;
        customColorToggle.isOn = true;
        foreach (PaintWithMouse pwm in painters)
        {
            pwm.enabled = true;
            pwm.SetInvertInput(true);
            pwm.drawOnPattern = false;
        }
    }

    public void StopBrush()
    {
        foreach (Image i in buttonImages) i.color = Color.white;
        buttonImages[4].color = highlightColor;
        foreach (PaintWithMouse pwm in painters) pwm.enabled = false;
    }
}
