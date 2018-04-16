using UnityEngine;
using System.Collections;

public class Score {
    public int SmashHitsCount = 0;
    public int SmashHitTotal = 0;
    public float BallInAirTime = 0;
    public float BallInAirTotal = 0;
    public int DynamicObjectsHit = 0;
    public int DynamicObjectTotal;

    public bool InHole = true;

    public string ShotText = "";
    public int ShotCount = 0;
    public int ShotTotal = 0;
    public int Par;

    private int oneStarMinScore, twoStarMinScore, threeStarMinScore = 0;

    public Score(StarScores minScores) {
        this.oneStarMinScore = minScores.OneStar;
        this.twoStarMinScore = minScores.TwoStar;
        this.threeStarMinScore = minScores.ThreeStar;
    }

    public int GetStarScore() {
        int score = CalcScore();

        if (score < oneStarMinScore)
            return 0;
        else if (score < twoStarMinScore)
            return 1;
        else if (score < threeStarMinScore)
            return 2;
        else
            return 3;
    }

    public int CalcScore() {
        int total = 0;
        SmashHitTotal = SmashHitsCount * 1000;
        BallInAirTotal = BallInAirTime * 2;
        DynamicObjectTotal = DynamicObjectsHit * 1000;

        if (InHole) {
            total += 1000;


            int dif = Par - ShotCount;
            if (dif < 0) {
                ShotText = "Par+" + Mathf.Abs(dif);
            } else {
                switch (dif) {
                    case 0: {
                            ShotText = "Par";
                            ShotTotal = 2000;
                            break;
                        }
                    case 1: {
                            ShotText = "Birdie";
                            ShotTotal = 5000;
                            break;
                        }
                    case 2:
                    default: {
                            ShotText = "Eagle";
                            ShotTotal = 7500;
                            break;
                        }
                }
            }
        }
        return total + SmashHitTotal + (int)BallInAirTotal + DynamicObjectTotal + ShotTotal;
    }
}
