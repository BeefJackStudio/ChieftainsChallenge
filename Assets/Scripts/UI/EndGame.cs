using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

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
		if(sepRef != null && onCompletedLevel != null) { sepRef.PlaySFX(onCompletedLevel); }
		StartCoroutine(EndGameAnimation(starAmount));
	}

	public IEnumerator EndGameAnimation(int starAmount) {
		isAnimating = true;
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
		if(LevelManager.CurrentLevel == null) {
			Debug.LogWarning("Could not submit level score to savemanager. Did you start from the Initialization level?");
			return;
		} 

		GameLevelSet nextLevel = LevelManager.CurrentLevel.GetNextLevel();
		if(nextLevel == null) {
			LevelManager.LoadScene("MainMenu");
		} else {
			LevelManager.LoadLevel(nextLevel);
		}
	}
}
