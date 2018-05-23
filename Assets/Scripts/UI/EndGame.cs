﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviourSingleton<EndGame> {

    [Header("Objects")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI cannonRewardText;
    public GameObject continueButton;
    public GameObject exitButton;

    [Header("Stars")]
	public List<GameObject> stars;
	public Sprite emptyImage;
	public Sprite acquiredImage;
	float animationDelay = 0.5f;
	[ReadOnly] 	public bool isAnimating = false;

	[Header("Sound")]
	[ReadOnly]	public SoundEffectsPlayer sepRef = null;
				public SoundEffectCollection onCompletedLevel;
				public SoundEffectCollection onStarEarned;

    private void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    [ContextMenu("End Game Test")]
	public void EndGameTest() {
		ShowEndGameUI(3);
	}

	public void ShowEndGameUI(int starAmount) {
		if(stars.Count < 3) {
			Debug.LogError("Could not find enough star sprites.");
			return;
		}

		sepRef = SoundEffectsPlayer.Instance;
		if(sepRef == null) {
			Debug.LogWarning("EndGame can't play sound effects.");
		}

		gameObject.SetActive(true);
		StartCoroutine(EndGameAnimation(starAmount));
	}

    public void ShowEndGameUICannon() {
        gameObject.SetActive(true);

        foreach (GameObject star in stars) {
            star.gameObject.SetActive(false);
        }

        bool nextLevel = IsNextLevelAvailable();
        cannonRewardText.text = nextLevel ? "Proceed to the next level!" : "Claim your rewards here!";
        cannonRewardText.gameObject.SetActive(true);

        continueButton.SetActive(true);
        exitButton.SetActive(false);
    }

    public void ShowEndGameFailCannon() {
        gameObject.SetActive(true);

        foreach (GameObject star in stars) {
            star.gameObject.SetActive(false);
        }

        title.text = "Level failed";

        bool nextLevel = IsNextLevelAvailable();
        cannonRewardText.text = "Only one shot per try. Again?";
        cannonRewardText.gameObject.SetActive(true);

        continueButton.SetActive(false);
        exitButton.SetActive(true);
    }

	public IEnumerator EndGameAnimation(int starAmount) {
		isAnimating = true;
		if(sepRef != null && onCompletedLevel != null) { 
			sepRef.PlaySFX(onCompletedLevel);
			yield return new WaitForSeconds(onCompletedLevel.GetLengthInSeconds());
		}

		for(int i = 0; i < starAmount; i++) {
			Animator an = stars[i].GetComponent<Animator>();
			if(an == null){ continue; }

			an.Play("AquiredAnimation");
			if(sepRef != null && onStarEarned != null) { sepRef.PlaySFX(onStarEarned); }
			yield return new WaitForSeconds(animationDelay);
		}
		isAnimating = false;
	}

	public void GoNextLevel() {
        bool nextLevel = IsNextLevelAvailable();

        if (LevelManager.Instance.CurrentLevel == null) {
			Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
			return;
		} 

		if(!nextLevel) {
			LevelManager.Instance.LoadScene("MainMenu");
		} else {
			LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel.GetNextLevel());
		}
	}

    public void QuitToMainMenu() {
        LevelManager.Instance.LoadScene("MainMenu");
    }

    private bool IsNextLevelAvailable() {
        if (LevelManager.Instance.CurrentLevel == null) {
            return false;
        }

        GameLevelSet nextLevel = LevelManager.Instance.CurrentLevel.GetNextLevel();
        if (nextLevel == null) {
            return false;
        } else {
            return true;
        }
    }
}
