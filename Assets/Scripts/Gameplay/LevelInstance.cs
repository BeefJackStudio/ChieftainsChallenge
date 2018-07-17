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
    private const float SHOOT_POWER_MULTIPLIER_CANNON = 45;

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

        OnNextTurn();

        if (useCannon) {
            TimeUtilities.ExecuteAfterDelay(TriggerNextTurn, 1, this);
        }
    }

    private void Update() {
		if(GetBall() == null || useCannon) { return; }

        if (Input.GetKeyDown(KeyCode.U)) {
            ShowEndGame();
        }

		if(GetBall().isSleeping && characterInstance != null) {
            Vector3 scale = characterInstance.transform.localScale;

            if (GetBall().IsAimingRight()) {
				characterInstance.transform.position = GetBall().slotLeft;
                characterInstance.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
			} else {
				characterInstance.transform.position = GetBall().slotRight;
                characterInstance.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
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
        MaskSelectionMenu.Instance.HideOpenButton();
        BallSelectionMenu.Instance.HideOpenButton();

        //start play animation
        if (!useCannon) {
            Animation a = characterInstance.GetComponentInChildren<Animation>();
            a.Play("AN_Golf_Swing", PlayMode.StopAll);

            //wait for seconds
            yield return new WaitForSeconds(animationDelayShoot);

            //shoot ball.
            GetBall().HitBall(ShootPower);

            yield return new WaitForSeconds(1);
            a.Play("AN_Base_Pose", PlayMode.StopAll);
        } else {
            GameBall ball = GetBall();
            ball.transform.position = CannonController.Instance.ballSpawnPoint.position;
            ball.gameObject.SetActive(true);
            ball.HitBall(ShootPower);

            CannonController.Instance.shotParticles.SetActive(true);
            CannonController.Instance.shotParticles.GetComponent<ParticleSystem>().Play(true);
        }

        yield return null;
	}

    public void TriggerNextTurn() {
        if (levelState == LevelState.ENDING) return;

        if (useCannon) {
            if (shotsFired != 0) {
                if (levelState != LevelState.ENDING) {
                    EndGame.Instance.ShowEndGameFailCannon();
                }
                return;
            }
        } else {
            if(shotsFired >= starThreshold0) {
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
		shotsFired++;
	}

    public void ApplyCharacterMask(CharacterMask mask) {
        characterInstance.GetComponent<CharacterSpriteHandler>().ApplyMask(mask);
        ShootingHUD.Instance.powerCurve = mask.powerCurve;
        ShootingHUD.Instance.powerTimeScale = mask.powerTimeScale;
        m_PowerMaskMultiplier = mask.powerMultiplier;
    }

    public void ShowEndGame() {
        if (levelState == LevelState.ENDING) { return; }
        Debug.Log("Game ending.");

        int stars = 3;

        if (!useCannon) {
            //calc stars
            List<int> thresholds = new List<int>() { starThreshold2, starThreshold1, starThreshold0 };
            for (int i = 0; i < thresholds.Count; i++) {
                if (shotsFired >= thresholds[i]) { stars--; }
            }

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

            GameObject unlockObject = null;
            if (SaveDataManager.Instance.UpdateItemClaimCount()) {
                unlockObject = SaveDataManager.Instance.UnlockRandomItem();
            }

            EndGame.Instance.ShowEndGameUI(stars, lastScore, unlockObject);
        } else {
            EndGame.Instance.ShowEndGameUICannon();
        }

        //set level ended.
        levelState = LevelState.ENDING;

        SaveDataManager.Instance.Save();
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
