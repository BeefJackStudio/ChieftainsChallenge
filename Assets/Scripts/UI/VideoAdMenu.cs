using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;

public class VideoAdMenu : MonoBehaviourSingleton<VideoAdMenu> {

#if UNITY_IOS
    private string gameId = "2659070";
#elif UNITY_ANDROID
    private string gameId = "2659072";
#endif

    public GameObject maxLivesGroup;
    public GameObject nonMaxLivesGroup;

    public GameObject adObject;

    public Button2D watchAdButton;
    public TextMeshProUGUI livesTimerText;
    public Button2D claimButton;

    private TextMeshProUGUI claimButtonText;

    private void Awake() {
        claimButtonText = claimButton.GetComponentInChildren<TextMeshProUGUI>();

        Instance = this;
        gameObject.SetActive(false);
    }

    private void Start() {
        watchAdButton.onClick.AddListener(WatchAd);
        claimButton.onClick.AddListener(ClaimLives);

        if (Advertisement.isSupported) {
            Advertisement.Initialize(gameId, true);
        }
    }

    private void OnEnable() {
        maxLivesGroup.SetActive(SaveDataManager.Instance.GetPotentialLivesToGain() == 0);
        nonMaxLivesGroup.SetActive(SaveDataManager.Instance.GetPotentialLivesToGain() != 0);
    }

    private void Update() {
        int potentialLivesToClaim = SaveDataManager.Instance.GetPotentialLivesToGain();
        int livesToClaim = Mathf.Clamp(SaveDataManager.Instance.GetLivesToGain(), 0, potentialLivesToClaim);

        if (livesToClaim == 0 || potentialLivesToClaim == 0) {
            claimButtonText.text = "No lives to claim";
            claimButton.interactable = false;
        } else {
            claimButtonText.text = "Claim " + livesToClaim + (livesToClaim == 1 ? " life" : " lives");
            claimButton.interactable = true;
        }

        if (potentialLivesToClaim == 0) {
            livesTimerText.text = "No lives to gain";
        } else if (livesToClaim >= potentialLivesToClaim) {
            livesTimerText.text = "Claim your lives now!";
        } else {
            TimeSpan timeUntilNextLife = SaveDataManager.Instance.GetTimespanUntilNextLife();
            int hours = timeUntilNextLife.Hours;
            int minutes = timeUntilNextLife.Minutes;

            StringBuilder sb = new StringBuilder();
            sb.Append("Next life in ");
            
            if(hours != 0) {
                sb.Append(hours);
                if(hours == 1) {
                    sb.Append(" hour ");
                }else {
                    sb.Append(" hours ");
                }
                sb.Append("and ");
            }

            sb.Append(minutes);
            if(minutes == 1) {
                sb.Append(" minute.");
            }else {
                sb.Append(" minutes.");
            }

            livesTimerText.text = sb.ToString();
        }
    }

    private void WatchAd() {
        const string RewardedPlacementId = "rewardedVideo";

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(RewardedPlacementId, options);
    }


    private void HandleShowResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                SaveDataManager.Instance.ModifyLifeCount(3);

                break;
            case ShowResult.Skipped:
            case ShowResult.Failed:
                break;
        }

        OnEnable();
    }

    private void ClaimLives() {
        SaveDataManager.Instance.UpdateLivesGained();
        gameObject.SetActive(false);
    }
}
