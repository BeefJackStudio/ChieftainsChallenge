using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SeedBed : MonoBehaviour {

    [System.Serializable]
    public class seedGroup {
        [SerializeField]
        public float weighting;

        [SerializeField]
        public AnimationCurve zDistribution;

        [SerializeField]
        public List<GameObject> prefabs;
    }

    [SerializeField]
    float spacingX = 4.0f;
    [SerializeField]
    float spacingZ = 2.0f;

    [SerializeField]
    float density = 0.05f;

    [SerializeField]
    List<seedGroup> groups = new List<seedGroup>();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Generate() {
        if (groups == null || groups.Count == 0) {
            return; // as nothing to generate
        }

        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;

        int nx = (int)(scale.x / spacingX);
        if (nx < 1) {
            nx = 1;
        }
        float nx2 = nx * 2;
        float dx = scale.x / nx2;

        int nz = (int)(scale.z / spacingZ);
        if (nz < 1) {
            nz = 1;
        }
        float nz2 = nz * 2;
        float dz = scale.z / nz2;

        int quantity = (int)((nx * nz * 2) * density);
        for (int i = 0; i < quantity; ++i) {
            int ix = Random.Range(0, nx * 2);
            int iz = Random.Range(0, nz) * 2;
            if ((ix & 0x1) > 0) {
                ++iz;
            }
            float x = (ix / nx2 - 0.5f) * scale.x + (0.5f + Random.Range(-0.25f, 0.25f)) * dx + pos.x;
            float z = (iz / nz2 - 0.5f) * scale.z + (0.5f + Random.Range(-0.25f, 0.25f)) * dz + pos.z;

            float distributionZ = (z - pos.z + scale.z * 0.5f) / scale.z;
            float sumWeighting = 0;
            for (int j = 0; j < groups.Count; ++j) {
                seedGroup group = groups[j];
                float p = group.zDistribution.Evaluate(distributionZ);
                sumWeighting += group.weighting * p;
            }

            float r = Random.Range(0, sumWeighting);
            float cumulativeWeighting = 0;
            for (int j = 0; j < groups.Count; ++j) {
                seedGroup group = groups[j];
                float p = group.zDistribution.Evaluate(distributionZ);
                cumulativeWeighting += group.weighting * p;
                if (r >= cumulativeWeighting) {
                    continue; // to consider next group
                }
                if (group.prefabs == null || group.prefabs.Count == 0) {
                    break; // as chosen group is empty (safeguard)
                }
                GameObject obj = Instantiate(group.prefabs[Random.Range(0, group.prefabs.Count)]);
                obj.transform.position += new Vector3(x, pos.y, z); // note - offset object y relative to transform y
                obj.transform.parent = transform;
                break; // as chosen object generated
            }
        }
    }
}

[CustomEditor(typeof(SeedBed))]
public class SeedBedEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SeedBed seedBed = (SeedBed)target;

        if (GUILayout.Button("Generate")) {
            if (seedBed.transform.childCount == 0) {
                seedBed.Generate();
            }
        }

        if (GUILayout.Button("Delete")) {
            for (int i = seedBed.transform.childCount - 1; i >= 0; --i) {
                DestroyImmediate(seedBed.transform.GetChild(i).gameObject);
            }
        }
    }
}
