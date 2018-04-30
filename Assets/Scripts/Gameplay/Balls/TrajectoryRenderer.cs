using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(GameBall))]
public class TrajectoryRenderer : MonoBehaviour {

	[Header("Status")]
	[ReadOnly]  public List<GameObject> trajectoryPoints;
	[ReadOnly]	public float timer = 0.0f;
	[ReadOnly]	private bool m_doRender = false;

	[Header("Config")]
    public int steps = 15;
    public int timeScale = 5;
    public GameObject pointPrefab;
    public float calculateEachSeconds = 0.01f;
	public Vector2 StartScale = new Vector3(1f, 1f, 1f);
	public Vector2 EndScale = new Vector3(0.2f, 0.2f, 0.2f);
    public Color color = Color.white;

	public void StartRender() {
		InstantiateRenderTrajectoryGameObjects(steps);
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
		Vector2[] trajectory = Plot(gameObject.GetComponent<Rigidbody2D>(), 
													gameObject.transform.position, 
													LevelInstance.Instance.ShootPower);
		RenderTrajectory(trajectory);
	}

    //https://tech.spaceapegames.com/2016/07/05/trajectory-prediction-with-unity-physics/
    public Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity) {
        Vector2[] results = new Vector2[steps];

        float timestep = (Time.fixedDeltaTime / Physics2D.velocityIterations) * timeScale;
        Vector2 gravityAccel = ((Physics2D.gravity * rigidbody.gravityScale) + LevelInstance.Instance.windForce) * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; ++i) {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }

    void InstantiateRenderTrajectoryGameObjects(int steps) {
		Vector3 scaleDecreasePerStep = StartScale - EndScale;
		scaleDecreasePerStep /= steps;

        for(int i = 0; i < steps; i++) {
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
