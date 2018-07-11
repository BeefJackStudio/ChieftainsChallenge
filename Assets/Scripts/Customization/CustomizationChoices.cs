using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CustomizationChoices : ScriptableObject {

    public CustomizationTypes customizationType;
    public string headerText;
    public List<GameObject> options;

}
