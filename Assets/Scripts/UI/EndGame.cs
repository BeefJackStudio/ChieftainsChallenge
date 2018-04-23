using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

	public List<GameObject> stars;
	public Sprite emptyImage;
	public Sprite acquiredImage;
	float animationDelay = 0.5f;
	[ReadOnly] public bool isAnimating = false;


	[ContextMenu("End Game Test")]
	public void EndGameTest() {
		ShowEndGameUI(3);
	}

	public void ShowEndGameUI(int starAmount) {
		if(stars.Count < 3) {
			Debug.LogError("Could not find enough star sprites.");
			return;
		}

		gameObject.SetActive(true);
		StartCoroutine(EndGameAnimation(starAmount));
	}

	public IEnumerator EndGameAnimation(int starAmount){
		isAnimating = true;
		for(int i = 0; i < starAmount; i++) {
			Animator an = stars[i].GetComponent<Animator>();
			if(an == null){ continue; }

			an.Play("AquiredAnimation");
			yield return new WaitForSeconds(animationDelay);
		}
		isAnimating = false;
	}
}
