using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CustomizationDatabase : ScriptableObject {

    public CustomizationChoices[] sectionMask;
    public CustomizationChoices[] sectionBall;
    public CustomizationChoices sectionSkin;
    public CustomizationChoices sectionParticle;

}
