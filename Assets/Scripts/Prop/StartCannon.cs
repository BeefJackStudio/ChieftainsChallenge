using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartCannon : MonoBehaviour {
    public GameObject TakeShot;
    public GameObject WindHUD;
    public RectTransform CannonWheel;
    public RectTransform wheelPin;
    public RectTransform wheelString;
    public Transform wheelEndPosX;
    public Transform cannonBarrel;
    public Transform ballAncer;
    public GameObject GunPowder;
    public Ball ball;

    public Animation[] tribesMen;
    public Animation cannonLighter;

    public ParticleSystem Smoke;

    public GameObject CannonUI;
    public GameObject CoalUI;
    public GameObject ArrowUI;
    public GameObject FireUI;

    public enum StartCannonState {
        Wait,
        PrepForFire,
        Fire
    };
    public StartCannonState state;

    public int lastTouch;

    ParticleSystem spawnedParticles;
    bool touchDown = false;
    float barrelAngle = 50;
    int powder;
    float fallowBallDelay;

    public ParticleSystem LightCannon;

    public AudioSource gunpowderAS;
    public AudioSource wheelAS;
    public AudioSource wickAS;
    public AudioSource boomAS;

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        spawnedParticles = null;
        lastTouch = 0;
        powder = 0;
        InputManager.inputStart += TouchDown;
        InputManager.inputFinished += TouchUp;

        NotificationsManager.OnCourseStart += NotificationsManager_OnCourseStart;

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnCourseStart -= NotificationsManager_OnCourseStart;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnCourseStart(GameManager.GameType thisGameType, int coursePar) {
        if (thisGameType != GameManager.GameType.Cannon) {
            DisableCannon();
            gameObject.SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update() {
        switch (state) {
            case StartCannonState.Wait: {
                    if (ArrowUI.activeSelf) {
                        Vector3 pos = ArrowUI.transform.position;
                        pos.x = Mathf.Lerp(pos.x, wheelEndPosX.position.x, Time.deltaTime * 3);
                        ArrowUI.transform.position = pos;
                    }
                    break;
                }
            case StartCannonState.PrepForFire: {
                    if (fallowBallDelay > 0) {
                        fallowBallDelay -= Time.deltaTime;
                        if (fallowBallDelay <= 0) {
                            FireCannon();
                            SoundManager.playSFX(boomAS);
                            LightCannon.Stop();
                        }
                        if (fallowBallDelay <= 1) {
                            if (!LightCannon.isPlaying) {
                                LightCannon.Play();
                                SoundManager.playSFX(wickAS);
                            }
                            Vector3 pos = Vector3.Lerp(new Vector3(-2.255f, 1.42f, 0), new Vector3(-0.6f, 2.2f, 0), 1 - (fallowBallDelay));
                            LightCannon.transform.position = pos;
                        }
                    }
                    break;
                }
            case StartCannonState.Fire: {
                    if (fallowBallDelay > 0) {
                        fallowBallDelay -= Time.deltaTime;
                        Color col = CannonWheel.GetComponent<Image>().color;
                        col.a = Mathf.Clamp01(col.a - Time.deltaTime);
                        CannonWheel.GetComponent<Image>().color = col;
                        wheelPin.GetComponent<Image>().color = col;
                        wheelString.GetComponent<Image>().color = col;

                        ArrowUI.GetComponent<Image>().color = col;
                        FireUI.GetComponent<Image>().color = col;
                        FireUI.transform.GetChild(0).GetComponent<Image>().color = col;


                        if (fallowBallDelay <= 0) {
                            GameManager.Instance.StartGame();
                            foreach (Animation anim in tribesMen) {
                                if (anim.transform.parent && anim.transform.parent.GetComponent<RandomCheering>() != null) {
                                    anim.transform.parent.GetComponent<RandomCheering>().enabled = true;
                                } else if (anim.transform.parent && anim.transform.parent.GetComponent<Player>() != null) {
                                    anim.transform.parent.GetComponent<Player>().state = Player.PlayerState.WaitForPlayer;
                                }
                                anim.CrossFade("chieftain_idle", 0.25f);
                            }
                            //TakeShot.SetActive(true);
                            WindHUD.SetActive(true);
                            CannonUI.SetActive(false);

                        }
                    }
                    break;
                }
        }
    }

    public void WheelTouch(int index) {
        Debug.Log("index: " + index);
        if (lastTouch != -1) {
            if (touchDown && state == StartCannonState.Wait) {
                Vector3 rot = CannonWheel.eulerAngles;
                if (index != 1 && index != 12) {
                    if (index < lastTouch) {
                        rot.z += 10;
                        barrelAngle += 2;
                        if (!wheelAS.isPlaying) {
                            SoundManager.playSFX(wheelAS);
                        }

                    } else if (index > lastTouch) {
                        rot.z -= 10;
                        barrelAngle -= 2;
                        if (!wheelAS.isPlaying) {
                            SoundManager.playSFX(wheelAS);
                        }
                    }
                } else {
                    if (index == 1 && lastTouch == 12 || index == 12 && lastTouch == 11) {
                        rot.z -= 10;
                        barrelAngle -= 2;
                        if (!wheelAS.isPlaying) {
                            SoundManager.playSFX(wheelAS);
                        }
                    } else if (index == 12 && lastTouch == 1 || index == 1 && lastTouch == 2) {
                        rot.z += 10;
                        barrelAngle += 2;
                        if (!wheelAS.isPlaying) {
                            SoundManager.playSFX(wheelAS);
                        }
                    }
                }
                lastTouch = index;
                CannonWheel.eulerAngles = rot;
                barrelAngle = Mathf.Clamp(barrelAngle, -40, 50);
                rot = cannonBarrel.eulerAngles;
                rot.z = barrelAngle;
                cannonBarrel.eulerAngles = rot;
                CoalUI.SetActive(false);
                FireUI.SetActive(true);
            }
        } else {
            lastTouch = index;
        }
    }

    public void SpawnGunPowder() {
        if (state == StartCannonState.Wait && barrelAngle.Aprox(50, .5f) && gameObject.activeSelf) {
            if (spawnedParticles == null) {
                GameObject obj = Instantiate(GunPowder) as GameObject;
                obj.transform.position = new Vector3(0.95f, 12.89f, 0);
                obj.transform.eulerAngles = new Vector3(90, 180, 0);
                spawnedParticles = obj.GetComponent<ParticleSystem>();
            }
            spawnedParticles.Play();
            powder++;
            ArrowUI.SetActive(true);
            if (!gunpowderAS.isPlaying) {
                SoundManager.playSFX(gunpowderAS);
            }
        }
    }

    public void PrepForFire() {
        if (state == StartCannonState.Wait) {
            if (cannonLighter.transform.parent && cannonLighter.transform.parent.GetComponent<RandomCheering>() != null) {
                cannonLighter.transform.parent.GetComponent<RandomCheering>().enabled = false;
            }
            cannonLighter.CrossFade("chieftain_activate_cannon", 0.25f);
            fallowBallDelay = 2;
            state = StartCannonState.PrepForFire;
        }
    }

    public void FireCannon() {
        if (state == StartCannonState.PrepForFire) {
            state = StartCannonState.Fire;
            ball.gameObject.SetActive(true);
            ball.Init();
            ball.UpdateForceAngle(barrelAngle + 40);
            ball.UpdatePower(Mathf.Clamp((float)powder * .5f, 0, 3));
            ball.transform.position = ballAncer.position;
            ball.ShootBall();
            foreach (Animation anim in tribesMen) {
                if (anim.transform.parent && anim.transform.parent.GetComponent<RandomCheering>() != null) {
                    anim.transform.parent.GetComponent<RandomCheering>().enabled = false;
                }
                anim.CrossFade("Character_CanonFall", 0.25f);
            }
            fallowBallDelay = 2;
            Smoke.Play();
        }

    }

    void TouchDown() {
        touchDown = true;
    }

    void TouchUp() {
        touchDown = false;
        lastTouch = -1;
    }

    public void DisableCannon() {
        WindHUD.SetActive(true);
        CannonWheel.gameObject.SetActive(false);
        CannonUI.SetActive(false);
    }
}
