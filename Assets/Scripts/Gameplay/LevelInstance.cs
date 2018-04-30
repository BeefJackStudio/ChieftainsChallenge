using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelState {
	INTRO,
	SHOOTING,
	ENDING
}

public class LevelInstance : MonoBehaviourSingleton<LevelInstance> {

    private const float SHOOT_POWER_MULTIPLIER = 50;
	
	//Note: see GameCamera.cs triggers the go to ingame.
	[ReadOnly] public LevelState levelState = LevelState.INTRO;
	private GameObject m_currentBall = null;

	[Header("Wind")]
	public bool enableWind = false;
	public float minWindForce = 0.1f;
	public float maxWindForce = 0.3f;
	public Vector2 windForce = Vector2.zero;

	[Header("Stars")]
	[ReadOnly] 													public int shotsFired = 0;
	[Tooltip("Amount of shots when we drop to two stars.")]		public int starThreshold2 = 4;
	[Tooltip("Amount of shots when we drop to one star.")]		public int starThreshold1 = 6;
	[Tooltip("Amount of shots when we drop to no stars.")]		public int starThreshold0 = 8;

	[Header("Character")]
	[ReadOnly] 	public GameObject characterInstance;
				public GameObject characterPrefab;
				public float animationDelayShoot = 2.1f;

	[Header("Music")]
	[ReadOnly]	public SoundMusicPlayer smpRef = null;
				public AudioClip backgroundMusic;

    [Header("Shooting")]
    [ReadOnly] public Vector2 shootAngle;
    [ReadOnly] public float normalizedShootPower = 0.5f;
    public Vector2 ShootPower { get { return shootAngle * (normalizedShootPower * SHOOT_POWER_MULTIPLIER); } }
    //public Vector2 ShootPower { get { return new Vector2(1, 1) * SHOOT_POWER_MULTIPLIER; } }

    public Action OnNextTurn = delegate { };

    private void Awake() {
        ResetShootingAngle();
    }

    private void Start() {
		smpRef = SoundMusicPlayer.Instance;

		if(smpRef != null) {
            if (backgroundMusic == null) {
                Debug.LogWarning("You did not set background music for this level. If there's already music playing it will continue.");
            } else {
                smpRef.PlayMusic(backgroundMusic);
            }
		}

        OnNextTurn();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            TriggerNextTurn();
        }
    }

    public void ShootBall() {
		StartCoroutine(ShootSequence());
    }

	public IEnumerator ShootSequence() {
		if(characterInstance == null) {

			yield return null;
		}

		//start play animation
		Animation a = characterInstance.GetComponentInChildren<Animation>();
		a.Play("AN_Golf_Swing", PlayMode.StopAll);

		//wait for seconds
		yield return new WaitForSeconds(animationDelayShoot);
		
		//shoot ball.
        GetBall().HitBall(ShootPower);
        ResetShootingAngle();

		yield return new WaitForSeconds(1);
		a.Play("AN_Base_Pose", PlayMode.StopAll);
		
		yield return null;
	}

    public void TriggerNextTurn() {
        levelState = LevelState.SHOOTING;
        RandomizeWind();
        OnNextTurn();

		if(characterInstance == null) { 
			characterInstance = Instantiate(characterPrefab);
		}
		GetBall().CalculateSlotLocations();
		characterInstance.transform.position = GetBall().slotLeft;
    }

    public void RandomizeWind() {
        windForce = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(minWindForce, maxWindForce);
    }

	public void OnShotFired() {
		shotsFired++;
	}

	public void ShowEndGame() {
		if(levelState == LevelState.ENDING) { return; }
		Debug.Log("Game ending.");

		//calc stars
		List<int> thresholds = new List<int>() {starThreshold2, starThreshold1, starThreshold0};
		int stars = 3;
		for(int i = 0; i < thresholds.Count; i++) {
			if(shotsFired >= thresholds[i]) { stars--; }
		}

		//show end game
		if(EndGame.Instance == null){
			Debug.LogError("No end game UI was assigned to LevelInstance!");
			return;
		}
        EndGame.Instance.ShowEndGameUI(stars);

		if(LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null) {
			SaveDataManager.Instance.SetLevelScore(LevelManager.Instance.CurrentLevel.scene, stars);
		} else {
			Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
		}
		
		//set level ended.
		levelState = LevelState.ENDING;
	}

    public void ResetShootingAngle() {
        shootAngle = new Vector2(0.5f, 0.5f).normalized;
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
