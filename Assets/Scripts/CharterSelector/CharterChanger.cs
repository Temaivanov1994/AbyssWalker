using UnityEngine;

public class CharterChanger : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] charterSO;
    [SerializeField] private ChartersDisplay chartersDisplay;

    private int currentIndex;
    private void Awake()
    {
        ChangeCharterSO(0);
    }

    public void ChangeCharterSO(int value)
    {
        currentIndex += value;

        if (currentIndex < 0)
        {
            currentIndex = charterSO.Length - 1;
        }
        else if (currentIndex > charterSO.Length - 1)
        {
            currentIndex = 0;
        }

        if (chartersDisplay != null)
        {
            chartersDisplay.CharterDisplay((CharterSO)charterSO[currentIndex]);
        }
    }
}
