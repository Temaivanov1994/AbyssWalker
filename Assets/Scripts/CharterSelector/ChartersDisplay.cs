using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChartersDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI charterName;
    [SerializeField] private TextMeshProUGUI charterDescription;
    [SerializeField] private Image charterImage;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject lockIcon;
   public void CharterDisplay(CharterSO charterSO)
    {
        charterName.text = charterSO.charterName;
        charterDescription.text = charterSO.charterDescription;
        charterImage.sprite = charterSO.charterImage;

        bool charterUnlocked = PlayerPrefs.GetInt("currentCharter", 0) >= charterSO.charterIndex;
        lockIcon.SetActive(!charterUnlocked);
        selectButton.interactable = charterUnlocked;
    }
}
