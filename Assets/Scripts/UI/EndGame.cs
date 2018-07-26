using System.Collections;
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
    public GameObject standardLevelParent;

    [Header("Unlocks")]
    public Image unlockImage;
    public Sprite questionSprite;
    public Sprite boxSprite;
    public TextMeshProUGUI unlockText;

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
		ShowEndGameUI(3, 0, false);
	}

	public void ShowEndGameUI(int starAmount, int lastStarAmount, bool unlock) {
		if(stars.Count < 3) {
			Debug.LogError("Could not find enough star sprites.");
			return;
		}

		sepRef = SoundEffectsPlayer.Instance;
		if(sepRef == null) {
			Debug.LogWarning("EndGame can't play sound effects.");
		}

        for (int i = 0; i < stars.Count; i++) {
            stars[i].GetComponent<Image>().sprite = (i + 1) <= lastStarAmount ? acquiredImage : emptyImage;
        }

        if(starAmount == 0) {
            title.text = "Level failed";
            
            if(IsNextLevelAvailable() && SaveDataManager.Instance.GetLevelScore(LevelManager.Instance.CurrentLevel.scene) == -1) {
                continueButton.SetActive(false);
            }else {
                continueButton.SetActive(true);
            }
        } else {
            title.text = "Level completed";
            continueButton.SetActive(true);
        }

		gameObject.SetActive(true);
        cannonRewardText.gameObject.SetActive(false);
        standardLevelParent.SetActive(true);

        if (!unlock) {
            unlockText.text = SaveDataManager.Instance.GetScoreLeftToUnlock() + " more perfect scores for unlock!";
            unlockImage.sprite = questionSprite;
        } else {
            unlockText.text = "Lootbox gained!";
            unlockImage.sprite = boxSprite;
        }
        
        StartCoroutine(EndGameAnimation(starAmount));
	}

    public void ShowEndGameUICannon() {
        gameObject.SetActive(true);

        standardLevelParent.SetActive(false);

        bool nextLevel = IsNextLevelAvailable();
        cannonRewardText.text = nextLevel ? "Proceed to the next level!" : "Claim your rewards here!";
        cannonRewardText.gameObject.SetActive(true);

        continueButton.SetActive(true);
        exitButton.SetActive(false);
    }

    public void ShowEndGameFailCannon() {
        gameObject.SetActive(true);

        standardLevelParent.SetActive(false);

        title.text = "Level failed";

        bool nextLevel = IsNextLevelAvailable();
        cannonRewardText.text = "Only one shot per try. Again?";
        cannonRewardText.gameObject.SetActive(true);

        continueButton.SetActive(false);
        exitButton.SetActive(true);
    }

	public IEnumerator EndGameAnimation(int starAmount) {
		isAnimating = true;

        RectTransform unlockTransform = unlockText.GetComponent<RectTransform>();
        RectTransform unlockImageTransform = unlockImage.GetComponent<RectTransform>();
        unlockTransform.localScale = Vector2.zero;
        unlockImageTransform.localScale = Vector2.zero;

        if (sepRef != null && onCompletedLevel != null) { 
			sepRef.PlaySFX(onCompletedLevel);
			yield return new WaitForSeconds(onCompletedLevel.GetLengthInSeconds());
		}

		for(int i = 0; i < starAmount; i++) {
            if (stars[i].GetComponent<Image>().sprite == acquiredImage) continue;

			Animator an = stars[i].GetComponent<Animator>();
			if(an == null){ continue; }

			an.Play("AquiredAnimation");
            stars[i].GetComponent<Image>().sprite = acquiredImage;
            if (sepRef != null && onStarEarned != null) { sepRef.PlaySFX(onStarEarned); }
			yield return new WaitForSeconds(animationDelay);
		}

        while(Vector2.Distance(unlockTransform.localScale, Vector2.one) >= 0.01f) {
            Vector2 newScale = Vector2.Lerp(unlockTransform.localScale, Vector2.one, 0.1f);
            unlockTransform.localScale = newScale;
            unlockImageTransform.localScale = newScale;
            yield return null;
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
