using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(GameBall))]
public class TrajectoryRenderer : MonoBehaviour {

	[Header("Status")]
    [ReadOnly]  public Vector2[] trajectoryPositions;
	[ReadOnly]	public float timer = 0.0f;
	[ReadOnly]	private bool m_doRender = false;
	private List<GameObject> trajectoryPoints;

	[Header("Config")]
    public bool includeWindInPrediction = false;
    public float predictionDuration = 0.25f;
    public GameObject pointPrefab;
    public float calculateEachSeconds = 0.01f;
	public Vector2 StartScale = new Vector3(1f, 1f, 1f);
	public Vector2 EndScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Color color = Color.white;

	public void StartRender() {
        trajectoryPoints = new List<GameObject>();
		InstantiateRenderTrajectoryGameObjects(gameObject.GetComponent<Rigidbody2D>());
		m_doRender = true;
	}

	public void StopRender() {
		m_doRender = false;
		timer = 0;
		CleanRenderTrajectoryGameObjects();
	}

	void Update () {
		if(!m_doRender) { return; }

		timer += Time.fixedDeltaTime;
		if(timer < calculateEachSeconds) { return; }

        //dirty getcomponent, we requirecomponent. but, todo: improve.
		timer = 0;
		trajectoryPositions = Plot(gameObject.GetComponent<Rigidbody2D>());
		RenderTrajectory(trajectoryPositions);
	}

    public Vector2[] Plot(Rigidbody2D rigidbody) {
        Vector2 angle = LevelInstance.Instance.shootAngle.normalized * 20;
        Vector2[] velocities;
        if(includeWindInPrediction && LevelInstance.Instance != null && LevelInstance.Instance.enableWind) {
            return TrajectoryTools.GetTrajectory(rigidbody, angle, ForceMode2D.Impulse, out velocities, LevelInstance.Instance.windForce, predictionDuration, false);
        } 

        return TrajectoryTools.GetTrajectory(rigidbody, angle, ForceMode2D.Impulse, out velocities, predictionDuration, false);
    }

    void InstantiateRenderTrajectoryGameObjects(Rigidbody2D rigidbody) {
        Vector2[] positions = Plot(rigidbody);

		Vector3 scaleDecreasePerStep = StartScale - EndScale;
		scaleDecreasePerStep /= positions.Length;

        for(int i = 0; i < positions.Length; i++) {
            GameObject rtgo = Instantiate(pointPrefab);
            rtgo.transform.SetParent(gameObject.transform);
            rtgo.transform.position = gameObject.transform.position;
			rtgo.transform.localScale = rtgo.transform.localScale - (scaleDecreasePerStep * i);
            rtgo.GetComponent<SpriteRenderer>().color = color;

            trajectoryPoints.Add(rtgo);
        }
    }

    void CleanRenderTrajectoryGameObjects() {
        foreach(GameObject rtgo in trajectoryPoints) {
            Destroy(rtgo);
        }

        trajectoryPoints.Clear();
    }

    void RenderTrajectory(Vector2[] plottedData) {
        if(plottedData.Length != trajectoryPoints.Count) { return; }

        for(int i = 0; i < plottedData.Length; i++) {
            trajectoryPoints[i].transform.position = plottedData[i];
        }
    }

    
}
