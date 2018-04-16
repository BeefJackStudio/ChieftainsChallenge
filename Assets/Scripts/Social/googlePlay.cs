#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Assets.Scripts.social
{
    class googlePlay : socialBase
    {
        public googlePlay():base()
        {
            messagePrefix = "GooglePlay: ";
            Leaderboards = new List<string>();
            Leaderboards.Add("CgkI3K2KiL0EEAIQAA");

        }

        public override void Authenticate()
        {

            //this is used for cloud saving

            //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            //.EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running.
            //.WithInvitationDelegate(< callback method >)
            // registers a callback for turn based match notifications received while the
            // game is not running.
            //.WithMatchDelegate(< callback method >)
            //.Build();
            //PlayGamesPlatform.InitializeInstance(config);


            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();
            PlayGamesPlatform.Instance.Authenticate(AuthenticationCallback);
        }
    }
}
#endif