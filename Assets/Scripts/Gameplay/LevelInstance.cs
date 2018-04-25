using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelState {
	intro,
	inGame,
	ended
}

public class LevelInstance : MonoBehaviour {
	
	//Note: see GameCamera.cs triggers the go to ingame.
	[ReadOnly] public LevelState levelState = LevelState.intro;
	private GameObject m_currentBall = null;

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

	[Header("Music")]
	[ReadOnly]	public SoundMusicPlayer smpRef = null;
				public AudioClip backgroundMusic;


	private void Start() {
		smpRef = SoundMusicPlayer.Instance;

		if(backgroundMusic == null) {
			Debug.LogWarning("You did not set background music for this level. If there's already music playing it will continue.");
			return;
		}

		if(smpRef != null) {
			smpRef.PlayMusic(backgroundMusic);
		}
	}

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

		if(LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null) {
			SaveDataManager.Instance.SetLevelScore(LevelManager.Instance.CurrentLevel.scene, stars);
		} else {
			Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
		}
		
		//set level ended.
		levelState = LevelState.ended;
	}

	public void SetBall(GameObject go) {
		if(go.GetComponent<GameBall>() == null) { return; }
		m_currentBall = go;
	}

	public void SetBall(GameBall gb) {
		m_currentBall = gb.gameObject;
	}

	public GameBall GetBall() {
		if(m_currentBall == null) { return null; }
		return m_currentBall.GetComponent<GameBall>();
	}
}
