using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetStars : MonoBehaviour {

    public Image StarOne, StarTwo, StarThree;

    public Sprite filled, empty;

    public void SetStarCount(int numStars) {
        Image[] stars = new Image[3] { StarOne, StarTwo, StarThree };

        for(int i = 0; i < numStars; i++) {
            stars[i].sprite = filled;
        }
    }
}
