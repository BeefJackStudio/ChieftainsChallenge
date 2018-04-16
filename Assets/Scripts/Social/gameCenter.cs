using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.social {
    class gameCenter : socialBase {
        public gameCenter() : base() {
            messagePrefix = "GameCenter: ";
            Leaderboards = new List<string>();
            Leaderboards.Add("SCORE");
        }

        public override void Authenticate() {
            UnityEngine.Social.localUser.Authenticate(AuthenticationCallback);
        }
    }
}
