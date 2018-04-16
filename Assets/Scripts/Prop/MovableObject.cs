using UnityEngine;
using System.Collections;

public class MovableObject : MonoBehaviour {
    public enum state {
        idle,
        moving
    }

    public state m_state;
    public bool moveAtStart;

    private Rigidbody2D m_body;

    bool tdown;

    // Use this for initialization
    void Start() {
        m_body = GetComponent<Rigidbody2D>();
        tdown = false;
        InputManager.inputStart += TouchDown;
        InputManager.inputFinished += TouchUP;
    }

    // Update is called once per frame
    void Update() {
        if (moveAtStart && GameManager.Instance.shotCount == 0 || !moveAtStart) {
            if (m_state == state.idle) {
                if (GetComponent<Renderer>().isVisible && Camera.main.GetComponent<BallCamera>().State == BallCamera.CameraStat.FreeCam) {
                    if (tdown) {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit2D[] hit;
                        hit = Physics2D.RaycastAll(ray.origin, ray.direction);
                        Debug.DrawLine(ray.origin, ray.origin + (ray.direction * 20), Color.white);

                        if (hit.Length > 0) {
                            foreach (RaycastHit2D thit in hit) {
                                if (thit.collider.gameObject == gameObject) {
                                    m_state = state.moving;
                                    m_body.constraints = RigidbodyConstraints2D.FreezeRotation;
                                    Camera.main.GetComponent<BallCamera>().HoldingObject = true;
                                }
                            }
                        }
                        if (m_state != state.moving) {
                            tdown = false;
                        }
                    }
                }
            } else if (m_state == state.moving) {
                if (!tdown) {
                    m_state = state.idle;
                    m_body.constraints = RigidbodyConstraints2D.FreezeRotation;
                    Camera.main.GetComponent<BallCamera>().HoldingObject = false;
                } else {
                    Vector3 tpos = InputManager.InputToScreenPoint(InputManager.positionLast);
                    tpos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                    Vector3 npos = Camera.main.ScreenToWorldPoint(tpos);
                    m_body.MovePosition(npos);
                }
            }
        }
    }

    public void TouchDown() {
        tdown = true;
    }

    public void TouchUP() {
        tdown = false;
        //Camera.main.GetComponent<BallCamera>().HoldingObject = false;
    }
}
