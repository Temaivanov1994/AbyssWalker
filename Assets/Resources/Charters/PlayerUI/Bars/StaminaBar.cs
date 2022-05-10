
using UnityEngine;
using UnityEngine.UI;


public class StaminaBar : MonoBehaviour
{

    public Slider staminaSlider;

    private void Awake()
    {
       
        staminaSlider = GetComponentInChildren<Slider>();

    }
    public void SetMaxStamina(int MaxStamina , float CurrentStamina)
    {
        staminaSlider.maxValue = MaxStamina;
        staminaSlider.value = CurrentStamina;


    }
    public void SetStamina(float CurrentStamina)
    {
        staminaSlider.value = CurrentStamina;

    }
}
