using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorseShaper : MonoBehaviour
{
    SkinnedMeshRenderer[] horseShapes;
    public Transform uiScale;

    public Slider[] breedSliders;
    public Toggle[] breedToggles;
    public TMP_Text[] breedPercentageTexts;
    float[] breedPercentages;
    bool[] includeBreed;
    public float[] breedAppropriateScales;

    public UiTranslator buttHighLowTranslator, legsLongShortTranslator, backLength, faceNoseShape, neckBase, neckLength, neckArch, headSize, buttSize;
    public List<UiGizmo> scaledGizmos;

    enum ShapeKey
    {
        WARMBLOOD,
        SHETLAND,
        ARABIAN,
        CHONK,
        DONKEY,
        LEGS_SHORT,
        LEGS_LONG,
        BUTT_LOW,
        BUTT_HIGH,
        BACK_SHORT,
        BACK_LONG,
        FACE_ROMAN_NOSE,
        FACE_DISHED,
        NECK_ARCHED,
        NECK_BASE_LOW,
        NECK_BASE_HIGH,
        NECK_SHORT,
        NECK_LONG,
        HEAD_SMALL,
        HEAD_BIG,
        BUTT_BIG,
        FOAL,

        NONE = -1
    }

    private void Awake()
    {
        horseShapes = GetComponentsInChildren<SkinnedMeshRenderer>();
        breedPercentages = new float[breedSliders.Length];
        includeBreed = new bool[breedSliders.Length];

        Utils.GetComponentsInChildrenRecursively<UiGizmo>(uiScale.transform, scaledGizmos);
    }

    private void Start()
    {
        for (int i = 0; i < breedPercentages.Length; i++) breedPercentages[i] = 0;
        for (int i = 0; i < includeBreed.Length; i++) includeBreed[i] = true;
        breedPercentages[0] = 100;

        buttHighLowTranslator.OnValueChanged += ButtHighLow;
        legsLongShortTranslator.OnValueChanged += LegsLongShort;
        backLength.OnValueChanged += BackLength;
        faceNoseShape.OnValueChanged += FaceNoseShape;
        neckBase.OnValueChanged += NeckBase;
        neckLength.OnValueChanged += NeckLength;
        neckArch.OnValueChanged += NeckArch;
        headSize.OnValueChanged += HeadSize;
        buttSize.OnValueChanged += ButtSize;

        UpdateBreedVisuals();
    }

    private void Update()
    {
        //transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }

    public void ToggleBreed(int breedIndex)
    {
        includeBreed[breedIndex] = breedToggles[breedIndex].isOn;

        if(!includeBreed[breedIndex]) breedPercentages[breedIndex] = 0;

        bool allOff = true;
        foreach (bool b in includeBreed) if (b) allOff = false;

        if(allOff)
        {
            for(int i = 0; i < breedToggles.Length; i++)
            {
                if(i == breedIndex)
                {
                    breedPercentages[i] = 0;
                    includeBreed[i] = false;
                }
                else
                {
                    breedPercentages[i] = 100;
                    includeBreed[i] = true;
                }
            }
        }

        //string debugString = "";
        //for (int i = 0; i < breedToggles.Length; i++) debugString += includeBreed[i] + " " + breedPercentages[i] + ", ";
        //Debug.Log(debugString);

        UpdateBreedPercentage(breedIndex);
    }

    public void SliderBreedPercentage(int breedIndex)
    {
        float setValue = breedSliders[breedIndex].value;
        breedPercentages[breedIndex] = setValue;

        UpdateBreedPercentage(breedIndex);
    }

    public void UpdateBreedPercentage(int breedIndex)
    {
        float remaining = 100f - breedPercentages[breedIndex];
        
        float oldRemainingSum = 0;
        for (int i = 0; i < breedSliders.Length; i++)
        {
            if (i == breedIndex) continue;
            if (!includeBreed[i]) breedPercentages[i] = 0;
            else if (breedPercentages[i] <= 0) breedPercentages[i] = 0.0001f;
            oldRemainingSum += breedPercentages[i];
        }

        for (int i = 0; i < breedSliders.Length; i++)
        {
            if (i == breedIndex) continue;
            breedPercentages[i] = breedPercentages[i] * remaining / oldRemainingSum;
            if (breedPercentages[i] < 0.005f) breedPercentages[i] = 0;
        }

        UpdateBreedVisuals();
    }

    void UpdateBreedVisuals()
    {
        int numActiveBreeds = 0;
        float breedAppropriateScale = 0;
        float modelScale = 1;
        for (int i = 0; i < breedSliders.Length; i++)
        {
            if (includeBreed[i]) numActiveBreeds++;
            breedToggles[i].Set(includeBreed[i], false);
            breedSliders[i].interactable = includeBreed[i];
            breedSliders[i].Set(breedPercentages[i]);
            breedPercentageTexts[i].text = breedPercentages[i].ToString("0.00") + "%";
            breedAppropriateScale += breedAppropriateScales[i] * breedPercentages[i] / 100f;
        }

        if (numActiveBreeds <= 1)
        {
            for (int i = 0; i < breedSliders.Length; i++)
            {
                breedSliders[i].interactable = false;
            }
        }

        modelScale = breedAppropriateScale;

        for(int h = 0; h< horseShapes.Length; h++)
        {
            for (int i = 0; i < breedPercentages.Length; i++) horseShapes[h].SetBlendShapeWeight(i, breedPercentages[i]);
            Vector3 rightSide = Vector3.one;
            Vector3 leftSide = new Vector3(1, 1, -1);
            if (h % 2 == 0) horseShapes[h].transform.localScale = rightSide * modelScale;
            else horseShapes[h].transform.localScale = leftSide * modelScale;
        }
        if (uiScale != null)
        {
            uiScale.localScale = Vector3.one * modelScale;
            //foreach(UiGizmo gizmo in scaledGizmos) foreach(MeshRenderer mesh in gizmo.meshesWithMaterialToUpdate) mesh.transform.localScale = Vector3.one / modelScale;
        }
    }

    void SetShapeKeyNormalized(float value, ShapeKey shapeKeyPositive, ShapeKey shapeKeyNegative = ShapeKey.NONE) //Value expected in range -1 to +1
    {
        float shapeKeyValue = value * 100f;

        if (shapeKeyValue > 0)
        {
            for (int h = 0; h < horseShapes.Length; h++)
            {
                horseShapes[h].SetBlendShapeWeight((int)shapeKeyPositive, shapeKeyValue);
                if (shapeKeyNegative != ShapeKey.NONE) horseShapes[h].SetBlendShapeWeight((int)shapeKeyNegative, 0);
            }
        }
        else
        {
            for (int h = 0; h < horseShapes.Length; h++)
            {
                horseShapes[h].SetBlendShapeWeight((int)shapeKeyPositive, 0);
                if (shapeKeyNegative != ShapeKey.NONE) horseShapes[h].SetBlendShapeWeight((int)shapeKeyNegative, shapeKeyValue * -1);
            }
        }
    }

    void ButtHighLow(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.BUTT_HIGH, ShapeKey.BUTT_LOW);
    }

    void LegsLongShort(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.LEGS_LONG, ShapeKey.LEGS_SHORT);
    }

    void BackLength(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.BACK_LONG, ShapeKey.BACK_SHORT);
    }

    void FaceNoseShape(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.FACE_ROMAN_NOSE, ShapeKey.FACE_DISHED);
    }

    void NeckLength(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.NECK_LONG, ShapeKey.NECK_SHORT);
    }

    void NeckArch(float value) //Value comes in range 0 to 1
    {
        SetShapeKeyNormalized(value, ShapeKey.NECK_ARCHED);
    }

    void NeckBase(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.NECK_BASE_LOW, ShapeKey.NECK_BASE_HIGH);
    }

    void HeadSize(float value) //Value comes in range -1 to +1
    {
        SetShapeKeyNormalized(value, ShapeKey.HEAD_BIG, ShapeKey.HEAD_SMALL);
    }

    void ButtSize(float value) //Value comes in range 0 to 1
    {
        SetShapeKeyNormalized(value, ShapeKey.BUTT_BIG);
    }
}
