using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

    public enum PlayerState {
        Follow,
        Idle
    }

    public Player player;
    public GameObject tribesman;
    private Animation animationComp;
    public PlayerState state;
    private float distToPlayer;

    public float FollowSpeed;          // speed the player is going
    public float FollowMaxSpeed;        // max speed the player can go
    public float FollowMaxAccel;        // accelaration for the player


    public SpriteRenderer Mask;
    public SpriteRenderer MaskOutline;
    public AnimationCurve slowDownSpline;
    // Use this for initialization
    void Start() {
        if (player == null) // find 'player' to follow
        {
            player = FindObjectOfType<Player>();
        }

        animationComp = new Animation()/*[tribesman.Length]*/;
        //    for(int i = 0; i < (tribesman.Length); i++)
        //   {
        animationComp = tribesman.GetComponentInChildren<Animation>();
        animationComp.CrossFade("chieftain_idle", 0.25f);
        //  }
        state = PlayerState.Idle;

        distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        // FollowSpeed = player.playerSpeed;          
        // FollowMaxSpeed = player.playerMaxSpeed;        
        // FollowMaxAccel = player.playerMaxAccel;      
    }

    void follow() {
        float distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        var pos = transform.position;
        if (pos.x < player.transform.position.x) {
            transform.localScale = new Vector3(2f, 2f, 2f);
            FollowSpeed += (Time.deltaTime * FollowMaxAccel); // * dir;
        } else if (pos.x > player.transform.position.x) {
            transform.localScale = new Vector3(2f, 2f, 2f);
            FollowSpeed -= (Time.deltaTime * FollowMaxAccel); // * dir;
        } else if (distToPlayer < -1.0f) {
            transform.localScale = new Vector3(-2f, 2f, 2f);
            FollowSpeed -= (Time.deltaTime * FollowMaxAccel);

        }

        //  float distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        float distToPlayery = Mathf.Abs(transform.position.y - player.transform.position.y);

        float sval = distToPlayer * 0.1f; //slowDownSpline.Evaluate(distToPlayer);


        // distToPlayer;

        float newMax = (sval * FollowMaxSpeed) * 2;
        //gameCamera.targetFOV = Mathf.Clamp(distToBall*10,35,75);

        //Debug.Log("Max Speed=" + playerMaxSpeed);
        FollowSpeed = Mathf.Clamp(FollowSpeed, -newMax, newMax);

        // audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01(SoundManager.SoundEffectVolume * (playerSpeed * 0.1f)), Time.deltaTime);

        var vel = (FollowSpeed * Time.deltaTime);
        //pos.x += vel;

        if (pos.y < player.transform.position.y + 1f) {
            pos.x += vel;
        }

        transform.position = pos;

        //  //update the animation speed based on the player speed
        //  for (int i = 0; i < (tribesman.Length); i++)
        //  {
        animationComp["chieftain_run"].speed = Mathf.Abs((vel * 10f).ClampNeg1());
        //   }


        distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        if (distToPlayer < 1.5f && /*Player.GetComponent<Rigidbody2D>().velocity.magnitude < 0.5f &&*/ distToPlayery < 0.1f) {
            // reset player 
            FollowSpeed = 0f;
            transform.localScale = new Vector3(2f, 2f, 2f);
            if (pos.x > player.transform.position.x) {

                pos.x -= 0.5f;
            }



            transform.position = pos;
            FollowSpeed = 0f;
            animationComp.CrossFade("chieftain_idle", 0.25f);
            //UpdateAnimation("chieftain_idle");
            pos.y = Mathf.Lerp(pos.y, player.transform.position.y, 2.0f);
            state = PlayerState.Idle;

        }

        //update the animation speed based on the player speed
    }



    /*  void UpdateAnimation(string animation)
      {
          for(int i = 0; i < (tribesman.Length); i++)
          {
              animationComp[i].CrossFade(animation, 0.25f);
          }
      }*/

    // Update is called once per frame
    void Update() {
        //ChangeDirection();
        //   foreach(GameObject t in tribesman)
        //  {
        switch (state) {
            case PlayerState.Idle: {
                    distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
                    //Debug.Log(distToPlayer + "updatedist");
                    if (distToPlayer > 5.0f || distToPlayer < -5.0f) {
                        animationComp.CrossFade("chieftain_run", 0.25f);
                        //  UpdateAnimation("chieftain_run");
                        //  for (int i = 0; i < tribesman.Length; i++)
                        // {                                
                        state = PlayerState.Follow;
                        //  }

                    }

                    break;
                }
            case PlayerState.Follow: {
                    distToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);


                    //  for (int i = 0; i < tribesman.Length; i++)
                    //  {
                    follow();
                    //  }


                    break;

                }

        }

    }

    //void ChangeDirection()
    //{
    //    //deadzone prevents player from rapidly changing directions when near ball
    //    var deadzone = 1f;

    //    var pos = transform.position;
    //    if (pos.x < player.transform.position.x - deadzone)
    //    {
    //        transform.localScale = new Vector3(2f, 2f, 2f);
    //    }
    //    else if (pos.x > player.transform.position.x + deadzone)
    //    {
    //        transform.localScale = new Vector3(-2f, 2f, 2f);
    //    }
    //}
}














