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
				public GenericLevelData levelData;
				public float animationDelayShoot = 2.1f;

	[Header("Music")]
	[ReadOnly]	public SoundMusicPlayer smpRef = null;
				public AudioClip backgroundMusic;

    [Header("Shooting")]
	[ReadOnly] public DirectionZone[] directionZones;
    [ReadOnly] public Vector2 shootAngle;
    [ReadOnly] public float normalizedShootPower = 0.5f;
    public Vector2 ShootPower { get { return shootAngle * (normalizedShootPower * SHOOT_POWER_MULTIPLIER); } }

    public Action OnNextTurn = delegate { };

    private void Awake() {
        ResetShootingAngle();
		FindDirectionZones();
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
		if(GetBall() == null) { return; }

        if (Input.GetKeyDown(KeyCode.U)) {
            TriggerNextTurn();
        }

		if(GetBall().isSleeping && characterInstance != null) { 
			if(GetBall().IsAimingRight()) {
				characterInstance.transform.position = GetBall().slotLeft;
				characterInstance.transform.rotation = new Quaternion(0, 0, 0, 0);
			} else {
				characterInstance.transform.position = GetBall().slotRight;
				characterInstance.transform.rotation = new Quaternion(0, 180, 0, 0);
			}
		}
    }

    public void ShootBall() {
		StartCoroutine(ShootSequence());
    }

	public IEnumerator ShootSequence() {
		if(characterInstance == null) {
			yield return null;
		}

        ShootingHUD.Instance.Hide();

		//start play animation
		Animation a = characterInstance.GetComponentInChildren<Animation>();
		a.Play("AN_Golf_Swing", PlayMode.StopAll);

		//wait for seconds
		yield return new WaitForSeconds(animationDelayShoot);
		
		//shoot ball.
        GetBall().HitBall(ShootPower);

		yield return new WaitForSeconds(1);
		a.Play("AN_Base_Pose", PlayMode.StopAll);
		
		yield return null;
	}

    public void TriggerNextTurn() {
        levelState = LevelState.SHOOTING;
        RandomizeWind();
		ResetShootingAngle();
        OnNextTurn();

		if(characterInstance == null) { 
			characterInstance = Instantiate(levelData.playerCharacterPrefab);
		}
		GetBall().CalculateSlotLocations();
		characterInstance.transform.position = GetBall().slotLeft;

        ShootingHUD.Instance.Show();
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
			Debug.LogWarning("Levelinstance could not find end game instance.");
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

        SaveDataManager.Instance.Save();
    }

    public void ResetShootingAngle() {
		DirectionZoneDirection direction = DirectionZoneDirection.RIGHT;

		foreach(DirectionZone dz in directionZones) {
			if(dz.isPositionInZone(GetBall().transform.position)) {
				direction = dz.shootingDirection;
				break;
			}
		}

		switch(direction) {
			case DirectionZoneDirection.RIGHT:
				shootAngle = new Vector2(0.5f, 0.5f).normalized;
				break;
			case DirectionZoneDirection.LEFT:
				shootAngle = new Vector2(-0.5f, 0.5f).normalized;
				break;
			default:
				shootAngle = new Vector2(0.5f, 0.5f).normalized;
				break;
		}

        
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

	public void FindDirectionZones() {
		directionZones = FindObjectsOfType<DirectionZone>();
	}
}
