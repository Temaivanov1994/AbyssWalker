using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewCharter", menuName ="ScriptableObjects/Charters")]
public class CharterSO : ScriptableObject
{
    public int charterIndex;
    public string charterName;
    public string charterDescription;
    public Sprite charterImage;
    public GameObject charterPrefab;
}
