using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float visibleDuration = 5f;
    [SerializeField] private bool alwaysVisible;
    [SerializeField] private GameObject floatingText;
    private float visibleTimeLeft = 0;
    public Slider healthSlider;




    private void Awake()
    {
        healthSlider = GetComponentInChildren<Slider>();
        floatingText = Resources.Load<GameObject>("Prefabs/FloatingText");
    }

    private void Start()
    {
        if (!alwaysVisible)
        {
            this.gameObject.SetActive(false);
        }
        
    }
    public void SetMaxHealth(int MaxHealth, int CurrentHealth)
    {
        healthSlider.maxValue = MaxHealth;
        healthSlider.value = CurrentHealth;
    }
    public void SetHealth(int Health)
    {
        if (!alwaysVisible)
        {
            this.gameObject.SetActive(true);
        }
        visibleTimeLeft = 0;
        healthSlider.value = Health;

    }

    public void ShowFloatingText(int damage, Transform CreatePoint)
    {
       
        GameObject _floatingText = Instantiate(floatingText , CreatePoint.position + Vector3.up*1.8f , Quaternion.identity ,CreatePoint);
        _floatingText.GetComponentInChildren<TextMeshPro>().text = damage.ToString();
        Destroy(_floatingText.gameObject, 2);
    }

    private void LateUpdate()
    {
        if (!alwaysVisible)
        {
            if (visibleTimeLeft >= visibleDuration)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                visibleTimeLeft += Time.deltaTime;
            }
        }
    }




}
