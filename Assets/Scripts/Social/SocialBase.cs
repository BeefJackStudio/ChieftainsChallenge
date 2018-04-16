using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.social {
    class socialBase {
        public bool LogDebugText = true;
        protected bool Authenticated = false;
        protected string messagePrefix = "SocialBase: ";
        protected List<string> Leaderboards;

        protected socialBase() { }

        public virtual void Authenticate() { }
        public void AuthenticationCallback(bool success) {
            if (success) {
                Log("Authenticated");
                Authenticated = true;
            } else {
                Log("Failed to authenticate");
                Authenticated = false;
            }
        }
        public void ShowLeaderboardUI() {
            if (Authenticated)
                Social.ShowLeaderboardUI();
            else
                Log("Not Authenticated");
        }

        public virtual void PostScore(long Score, int ScoreboardRef) {
            if (Authenticated) {
                if (Leaderboards.Count >= ScoreboardRef) {
                    Log("Posting " + Score + " to " + Leaderboards[ScoreboardRef]);
                    Social.ReportScore(Score, Leaderboards[ScoreboardRef], ScoreSubmissionCheck);
                } else {
                    Log("Invalid Scoreboard referance: " + ScoreboardRef);
                }

            } else {
                Log("Not Authenticated");
            }
        }

        void ScoreSubmissionCheck(bool result) {
            if (result)
                Log("score submission successful");
            else
                Log("score submission failed");
        }

        public void Log(string message) {
            if (LogDebugText)
                DebugStreamer.AddMessage(messagePrefix + message);
        }
    }
}
