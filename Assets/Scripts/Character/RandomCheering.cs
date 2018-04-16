using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//make the character play his cheer animation at random intervals
public class RandomCheering : MonoBehaviour {
    private Animation anim;
    private float deley;

    private bool mIfChieftainCheeringAnim = false;
    private bool mIfChieftainIdleAnim = false;

    public List<AudioClip> Cheers;

    // Use this for initialization
    void Start() {
        //start the character of as idle
        anim = GetComponentInChildren<Animation>();

        mIfChieftainCheeringAnim = (anim.GetClip("chieftain_cheering") != null);
        mIfChieftainIdleAnim = (anim.GetClip("chieftain_idle") != null);

        if (mIfChieftainIdleAnim) {
            anim.CrossFade("chieftain_idle", 0.0f);
        }

        //set the random wait time
        deley = Random.Range(5, 15);
    }

    // Update is called once per frame
    void Update() {
        //if were waiting for cheer
        if (deley > 0) {
            //dec the delay time
            deley -= Time.deltaTime;
            if (deley <= 0) {
                //if we hit zero start the cheers and set a new wait time
                if (mIfChieftainCheeringAnim) {
                    anim.CrossFade("chieftain_cheering", 0.5f);
                }

                if (Cheers != null && Cheers.Count > 0) // at least one SFX present
                {
                    SoundManager.playSFXOneShot(GetComponent<AudioSource>(), Cheers[Random.Range(0, Cheers.Count)]);
                }
                deley = Random.Range(5, 15);
            }
        }

        //if were not cheering play the idle
        if (!anim.IsPlaying("chieftain_cheering")) {
            if (mIfChieftainIdleAnim) {
                anim.CrossFade("chieftain_idle", 0.5f);
            }
        }
    }
}
