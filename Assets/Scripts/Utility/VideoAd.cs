using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;

public class VideoAd : MonoBehaviourSingleton<VideoAd> {

#if UNITY_IOS
    private string gameId = "2659070";
#elif UNITY_ANDROID
    private string gameId = "2659072";
#endif

    private Action m_OnComplete;
    private Action m_OnFail;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (Advertisement.isSupported) {
            Advertisement.Initialize(gameId, true);
        }
    }

    public void WatchAd(Action onComplete, Action onFail) {
        m_OnComplete = onComplete;
        m_OnFail = onFail;

        const string RewardedPlacementId = "rewardedVideo";

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(RewardedPlacementId, options);
    }


    private void HandleShowResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                if(m_OnComplete != null) m_OnComplete();
                break;
            case ShowResult.Skipped:
            case ShowResult.Failed:
                if (m_OnFail != null) m_OnFail();
                break;
        }
    }
}
