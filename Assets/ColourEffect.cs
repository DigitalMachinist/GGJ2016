using UnityEngine;

public struct ColourBenefit
{
    public float RegenRateCoeff;
    public float ActionRateCoeff;
    public float EnergyRateCoeff;

    public ColourBenefit( float regenRate, float actionRate, float energyRate )
    {
        RegenRateCoeff = regenRate;
        ActionRateCoeff = actionRate;
        EnergyRateCoeff = energyRate;
    }
}

public class ColourEffect : MonoBehaviour
{
    public AnimationCurve RegenRateWrtRed;
    public AnimationCurve ActionRateWrtGreen;
    public AnimationCurve EnergyRateWrtBlue;
}
