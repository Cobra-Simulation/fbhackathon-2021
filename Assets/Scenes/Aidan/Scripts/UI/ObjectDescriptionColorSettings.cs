using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectDescriptionColorSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private Image descriptionBackground = null;
    [SerializeField] private FireExtinguisherType extinguisherType = FireExtinguisherType.NOT_FIRE_EXTINGUISHER;
    [SerializeField] private Color defaultTextColor = Color.white;
    [SerializeField] private Color defaultBackgroundColor = Color.black;
    [SerializeField] private Color waterTextColor = Color.white;
    [SerializeField] private Color waterBackgroundColor = Color.black;
    [SerializeField] private Color afffFoamTextColor = Color.white;
    [SerializeField] private Color afffFoamBackgroundColor = Color.black;
    [SerializeField] private Color carbonDioxideTextColor = Color.white;
    [SerializeField] private Color carbonDioxideBackgroundColor = Color.black;
    [SerializeField] private Color abcPowderTextColor = Color.white;
    [SerializeField] private Color abcPowderBackgroundColor = Color.black;
    [SerializeField] private Color deionisedWaterMistTextColor = Color.white;
    [SerializeField] private Color deionisedWaterMistBackgroundColor = Color.black;
    [SerializeField] private Color wetChemicalTextColor = Color.white;
    [SerializeField] private Color wetChemicalBackgroundColor = Color.black;

    private void OnValidate()
    {
        if (!descriptionText || !descriptionBackground) return;

        switch (extinguisherType)
        {
            case FireExtinguisherType.NOT_FIRE_EXTINGUISHER:
                SetColors(defaultTextColor, defaultBackgroundColor);
                break;
            case FireExtinguisherType.WATER:
                SetColors(waterTextColor, waterBackgroundColor);
                break;
            case FireExtinguisherType.AFFF_FOAM:
                SetColors(afffFoamTextColor, afffFoamBackgroundColor);
                break;
            case FireExtinguisherType.CARBON_DIOXIDE:
                SetColors(carbonDioxideTextColor, carbonDioxideBackgroundColor);
                break;
            case FireExtinguisherType.ABC_POWDER:
                SetColors(abcPowderTextColor, abcPowderBackgroundColor);
                break;
            case FireExtinguisherType.DEIONISED_WATER_MIST:
                SetColors(deionisedWaterMistTextColor, deionisedWaterMistBackgroundColor);
                break;
            case FireExtinguisherType.WET_CHEMICAL:
                SetColors(wetChemicalTextColor, wetChemicalBackgroundColor);
                break;
            default:
                break;
        }
    }

    private void SetColors(Color textColor, Color backgroundColor)
    {
        descriptionText.color = textColor;
        descriptionBackground.color = backgroundColor;
    }
}
