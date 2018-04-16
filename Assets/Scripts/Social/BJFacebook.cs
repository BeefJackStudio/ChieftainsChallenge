using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using System;

public class BJFacebook : MonoBehaviour {

    // Use this for initialization
    void Awake() {
        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(SetInit, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    // Update is called once per frame
    void Update() {

    }

    //Callback for facebook initalization
    private void SetInit() {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            if (FB.IsLoggedIn) {
                Log("Already logged in");
                OnLoggedIn();
            }
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }

    }

    private void OnHideUnity(bool isGameShown) {
        Log("OnHideUnity");
        if (!isGameShown) {
            // pause the game - we will need to hide                                             
            Time.timeScale = 0;
        } else {
            // start the game back up - we're getting focus again                                
            Time.timeScale = 1;
        }
    }

    //------------------------------------

    //Login to facebook

    public void LoginToFacebook() {
        if (!FB.IsLoggedIn) {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, AuthCallback);
            //FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" };, AuthCallback);
        } else {
            FB.LogOut();
        }

    }

    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn) {
            OnLoggedIn();
        } else {
            Debug.Log("User cancelled login");
        }
    }

    void OnLoggedIn() {
        // AccessToken class will have session details
        var aToken = AccessToken.CurrentAccessToken;
        // Print current access token's User ID
        Debug.Log(aToken.UserId);
        // Print current access token's granted permissions
        foreach (string perm in aToken.Permissions) {
            Debug.Log(perm);
        }
    }
    //------------------------------------

    //Post message onto time line

    public void PostMessage() {
        if (FB.IsLoggedIn) {
            onBragClicked();
        }
    }

    private void onBragClicked() {
        Log("onBragClicked");
        FB.FeedShare(
                linkCaption: "This is to test messaging",
                picture: null,
                linkName: "Checkout this cool game!",
                link: new Uri("http://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + (FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "guest"))
                );
    }

    //------------------------------------

    //Invite frends to your app
    public void RequestFriends() {
        if (FB.IsLoggedIn) {
            onChallengeClicked();
        }
    }

    private void onChallengeClicked() {

        //FB.AppRequest("Test Message", null, null, null, null, "Test2", "Testing Testing");
        //FB.Mobile.AppInvite(new Uri("https://fb.me/810530068992919"), null, null);
        FB.AppRequest(
         "Come play this great game!",
         null, null, null, null, null, "Come and Play This Game!",
         delegate (IAppRequestResult result) {
             Debug.Log(result.RawResult);
         }
        );

    }

    private void Log(string message) {
        DebugStreamer.AddMessage("Facebook: " + message);
    }


    //------------------------------------

}
