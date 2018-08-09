using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenericLevelData : ScriptableObject {
    public int gameSpeed = 2;
    public GameObject playerCharacterPrefab;
    public GameObject playerAppearParticle;
    public GameObject trajectoryRendererPrefab;
}
