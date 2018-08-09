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

    private const float SHOOT_POWER_MULTIPLIER = 30;
    private const float SHOOT_POWER_MULTIPLIER_CANNON = 40;

    public bool useCannon = false;
	
	[ReadOnly] public LevelState levelState = LevelState.INTRO;
	private GameObject m_CurrentBall = null;
    private GameObject m_BallToDelete = null;

	[Header("Wind")]
	public bool enableWind = false;
	public float minWindForce = 0.1f;
	public float maxWindForce = 0.3f;
	public Vector2 windForce = Vector2.zero;

	[Header("Stars")]
	[ReadOnly] 													public int currentShot = 0;
	[Tooltip("Amount of shots when we drop to two stars.")]		public int starThreshold2 = 4;
	[Tooltip("Amount of shots when we drop to one star.")]		public int starThreshold1 = 6;
	[Tooltip("Amount of shots when we drop to no stars.")]		public int starThreshold0 = 8;

	[Header("Character")]
	[ReadOnly] 	public GameObject characterInstance;
				public GenericLevelData levelData;
				private float shotDelay = 0.5f;

	[Header("Music")]
	[ReadOnly]	public SoundMusicPlayer smpRef = null;
				public AudioClip backgroundMusic;

    [Header("Shooting")]
	[ReadOnly] public DirectionZone[] directionZones;
    [ReadOnly] public Vector2 shootAngle;
    [ReadOnly] public float normalizedShootPower = 0.5f;
    private float m_PowerMaskMultiplier = 1;
    public Vector2 ShootPower { get { return shootAngle * (normalizedShootPower * (useCannon ? SHOOT_POWER_MULTIPLIER_CANNON : SHOOT_POWER_MULTIPLIER) * m_PowerMaskMultiplier); } }
    //public Vector2 ShootPower { get { return new Vector2(0, 1) * SHOOT_POWER_MULTIPLIER; } }

    public Action OnNextTurn = delegate { };

    private GameObject m_PlayerDisappearParticle;
    private GameObject m_PlayerAppearParticle;

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

        TrajectoryRenderer trajectoryRenderer = Instantiate(levelData.trajectoryRendererPrefab).GetComponent<TrajectoryRenderer>();

        if (useCannon) {
            LevelManager.Instance.WaitForScenesLoaded(() => {
                TimeUtilities.ExecuteAfterDelay(TriggerNextTurn, 1, this);
            });

            trajectoryRenderer.transform.SetParent(CannonController.Instance.trajectoryLocation, false);
            trajectoryRenderer.transform.localPosition = Vector2.zero;
        } else {
            Destroy(CannonController.Instance.gameObject);
        }
    }

    private void WaitForDoneLoading() {

    }

    private void Update() {
        GameBall ball = GetBall();

        if (ball == null || useCannon) { return; }

        if (Input.GetKeyDown(KeyCode.U)) {
            ShowEndGame();
        }

		if(ball.isSleeping && characterInstance != null) {
            Vector3 scale = characterInstance.transform.localScale;

            if (ball.IsAimingRight()) {
				characterInstance.transform.position = ball.slotLeft;
                characterInstance.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
			} else {
				characterInstance.transform.position = ball.slotRight;
                characterInstance.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
            }

            TrajectoryRenderer.Instance.transform.position = ball.transform.position;
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
        MaskSelectionMenu.Instance.HideOpenButton();
        BallSelectionMenu.Instance.HideOpenButton();

        //start play animation
        if (!useCannon) {
            Animation a = characterInstance.GetComponentInChildren<Animation>();
            a.Play("AN_Golf_Swing", PlayMode.StopAll);
            a.PlayQueued("AN_Base_Pose");

            //wait for seconds
            yield return new WaitForSeconds(shotDelay);

            //shoot ball.
            GetBall().HitBall(ShootPower);
        } else {
            GameBall ball = GetBall();
            ball.transform.position = CannonController.Instance.ballSpawnPoint.position;
            ball.gameObject.SetActive(true);
            ball.SetGravityScale(ball.GetComponent<Rigidbody2D>().gravityScale);
            ball.HitBall(ShootPower);

            CannonController.Instance.shotParticles.SetActive(true);
            CannonController.Instance.shotParticles.GetComponent<ParticleSystem>().Play(true);
        }

        yield return null;
	}

    public void TriggerNextTurn() {
        if (levelState == LevelState.ENDING) return;

        LevelManager.Instance.WaitForScenesLoaded(DoNextTurn);
    }

    private void DoNextTurn() {
        currentShot++;

        if (useCannon) {
            if (currentShot != 1) {
                if (levelState != LevelState.ENDING) {
                    EndGame.Instance.ShowEndGameFailCannon();
                }
                return;
            }else {
                ApplyCharacterMask(CustomizationSelected.woodenMask.Obj);
            }
        } else {
            if (currentShot >= starThreshold0) {
                SaveDataManager.Instance.ModifyLifeCount(-1);
                ShowEndGame();
                return;
            }

            if (characterInstance == null) {
                characterInstance = Instantiate(levelData.playerCharacterPrefab);
                ApplyCharacterMask(CustomizationSelected.woodenMask.Obj);
            } else {
                if (m_PlayerDisappearParticle != null) {
                    Destroy(m_PlayerDisappearParticle);
                }

                m_PlayerDisappearParticle = Instantiate(levelData.playerAppearParticle, characterInstance.transform.position, Quaternion.identity);
            }

            GetBall().CalculateSlotLocations();
            characterInstance.transform.position = GetBall().slotLeft;

            if (m_PlayerAppearParticle != null) {
                Destroy(m_PlayerAppearParticle);
            }

            m_PlayerAppearParticle = Instantiate(levelData.playerAppearParticle, characterInstance.transform.position, Quaternion.identity);
            MaskSelectionMenu.Instance.ShowOpenButton();

            int stars = 3;
            int shotsLeft = 0;
            GetCurrentScoreStats(out stars, out shotsLeft);
            TurnsLeftHUD.Instance.StartSequence(stars - 1, shotsLeft);
        }

        ShootingHUD.Instance.Show();
        BallSelectionMenu.Instance.ShowOpenButton();

        levelState = LevelState.SHOOTING;
        RandomizeWind();
        ResetShootingAngle();
        OnNextTurn();
    }

    public void RandomizeWind() {
        windForce = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(minWindForce, maxWindForce);
    }

	public void OnShotFired() {

	}

    public void ApplyCharacterMask(CharacterMask mask) {
        if(mask == null) {
            Debug.LogError("No mask selected!");
            return;
        }
        if(characterInstance != null) characterInstance.GetComponent<CharacterSpriteHandler>().ApplyMask(mask);
        if(ShootingHUD.Instance != null) ShootingHUD.Instance.ApplyMask(mask);
        m_PowerMaskMultiplier = mask.powerMultiplier;
    }

    public void ShowEndGame() {
        if (levelState == LevelState.ENDING) { return; }
        Debug.Log("Game ending.");

        int stars = 3;
        int shotsLeft = 0;

        if (!useCannon) {
            //calc stars
            GetCurrentScoreStats(out stars, out shotsLeft);

            int lastScore = SaveDataManager.Instance.GetLevelScore(LevelManager.Instance.CurrentLevel.scene);
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null) {
                SaveDataManager.Instance.SetLevelScore(LevelManager.Instance.CurrentLevel.scene, stars);
            } else {
                Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
            }

            //show end game
            if (EndGame.Instance == null) {
                Debug.LogWarning("Levelinstance could not find end game instance.");
                return;
            }

            EndGame.Instance.ShowEndGameUI(stars, lastScore, SaveDataManager.Instance.UpdateItemClaimCount());
        } else {
            EndGame.Instance.ShowEndGameUICannon();
        }

        //set level ended.
        levelState = LevelState.ENDING;

        SaveDataManager.Instance.Save();
    }

    private void GetCurrentScoreStats(out int stars, out int shotsLeft) {
        stars = 3;

        List<int> thresholds = new List<int>() { starThreshold2, starThreshold1, starThreshold0 };
        shotsLeft = thresholds[0] - currentShot;
        for (int i = 0; i < thresholds.Count; i++) {
            if (currentShot >= thresholds[i]) {
                stars--;
                if (stars == 0) break;
                shotsLeft = thresholds[i + 1] - currentShot;
            }
        }
    }

    public void ResetShootingAngle() {
        if (useCannon) {
            shootAngle = new Vector2(0.5f, 0.5f).normalized;
            return;
        }

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

	public void SetBall(GameBall gb) {
        if (!useCannon) {
            Vector3 pos = gb.transform.position;
            if (m_CurrentBall != null) {
                m_BallToDelete = m_CurrentBall;
                pos = m_BallToDelete.transform.position;
                Destroy(m_BallToDelete);
                gb.StartSleepRoutine(true);
            }

            gb.transform.position = pos;
            gb.GetComponent<GameBall>().CalculateSlotLocations();

            TimeUtilities.ExecuteAfterDelay(() => {
                m_CurrentBall = gb.gameObject;
            }, 0.05f, this);

        } else {
            m_CurrentBall = gb.gameObject;
            gb.gameObject.SetActive(false);
        }
	}

	public GameBall GetBall() {
		if(m_CurrentBall == null) { return null; }
		return m_CurrentBall.GetComponent<GameBall>();
	}

	public void FindDirectionZones() {
		directionZones = FindObjectsOfType<DirectionZone>();
	}
}
