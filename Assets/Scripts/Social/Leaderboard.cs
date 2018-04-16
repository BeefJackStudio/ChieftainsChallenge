using Assets.Scripts.social;
using UnityEngine;

public class Leaderboard : MonoBehaviour {

    socialBase current;
    void Awake() {
        //dont destroy the game object that this is attached too
        DontDestroyOnLoad(transform.gameObject);
    }
    // Use this for initialization
    void Start() {

#if UNITY_EDITOR
        current = null;
#elif UNITY_IPHONE
        current = new gameCenter();
#elif UNITY_ANDROID
        current = new googlePlay();
#endif
        if (current != null)
            current.Authenticate();
    }

    public void RequestLeaderboardUI() {
        if (current != null)
            current.ShowLeaderboardUI();
    }

    public void UI_TEST_PostCore() {
        if (current != null)
            current.PostScore(20, 0);
    }


    public void PostScore(long score, int leaderboardRef) {
        if (current != null)
            current.PostScore(score, leaderboardRef);
    }



}
