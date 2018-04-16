using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
    public ParticleSystem explod;

    private float death;
    private Object particle;

    GameManager gm;

    // Use this for initialization
    void Start() {
        death = 0;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (death > 0) {
            death -= Time.deltaTime;
            if (death <= 0) {
                Destroy(gameObject);
                Destroy(particle);
                if (transform.parent) {
                    Destroy(transform.parent.gameObject);
                }
                gm.score.DynamicObjectsHit++;
            }
        }
    }

    //When the object is hit change it's state
    void OnCollisionEnter2D(Collision2D collision) {
        particle = GameObject.Instantiate(explod, transform.position, transform.rotation);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        death = 3;
        SoundManager.playSFX(GetComponent<AudioSource>());
    }
}
