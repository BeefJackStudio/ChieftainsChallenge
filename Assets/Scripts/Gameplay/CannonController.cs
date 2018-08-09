using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviourSingleton<CannonController> {

    public Transform ballSpawnPoint;
    public Transform cannonPiece;
    public GameObject shotParticles;
    public Transform trajectoryLocation;

    private LevelInstance m_LevelInstance;

    private void Start() {
        m_LevelInstance = LevelInstance.Instance;
    }

    private void Update() {
        if (!m_LevelInstance.useCannon) return;

        cannonPiece.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, m_LevelInstance.shootAngle) + 50);
    }
}
