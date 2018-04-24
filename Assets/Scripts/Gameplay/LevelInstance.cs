using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelState{
	inGame,
	ended
}

public class LevelInstance : MonoBehaviour {
	
	[ReadOnly] public LevelState levelState = LevelState.inGame;

	[Header("Wind")]
	public bool enableWind = false;
	public float minWindForce = 0.1f;
	public float maxWindForce = 0.3f;
	public Vector2 windDirection = Vector2.zero;

	[Header("Stars")]
	[ReadOnly] 													public int shotsFired = 0;
	[Tooltip("Amount of shots when we drop to two stars.")]		public int starThreshold2 = 4;
	[Tooltip("Amount of shots when we drop to one star.")]		public int starThreshold1 = 6;
	[Tooltip("Amount of shots when we drop to no stars.")]		public int starThreshold0 = 8;

	[Header("UI")]
	public EndGame endGameUI = null;

	public float GetRandomWindForce() {
		return Random.Range(minWindForce, maxWindForce);
	}

	public void OnShotFired() {
		shotsFired++;
	}

	public void EndGame() {
		if(levelState == LevelState.ended) { return; }
		Debug.Log("Game ending.");

		//calc stars
		List<int> thresholds = new List<int>() {starThreshold2, starThreshold1, starThreshold0};
		int stars = 3;
		for(int i = 0; i < thresholds.Count; i++) {
			if(shotsFired >= thresholds[i]) { stars--; }
		}

		//show end game
		if(endGameUI == null){
			Debug.LogError("No end game UI was assigned to LevelInstance!");
			return;
		}
		endGameUI.ShowEndGameUI(stars);
		if(LevelManager.CurrentLevel != null) {
			SaveDataManager.Instance.SetLevelScore(LevelManager.CurrentLevel.scene, stars);
		} else {
			Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
		}
		
		//set level ended.
		levelState = LevelState.ended;
	}
}
