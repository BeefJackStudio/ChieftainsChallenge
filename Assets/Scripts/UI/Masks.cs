using UnityEngine;
using System.Collections.Generic;
using System;

public class Masks : MonoBehaviour {

    public GameObject MainMenuPanel;
    public GameObject MasksPanel;

    public GameObject Walls;
    public List<Rigidbody2D> Ancors;
    public List<RectTransform> Wheels;

    public int numPages = 2;
    private int currPage = 0;

    private bool animating = false;
    private SwipeDetection.SwipeDirection animationDirection;

    private float animationdelta = 0.0f;
    private static float pageWidth = 20.5f;
    private static float speed = 4.0f;

    public static event Action<int> PageChange;

    void Start() {
        SwipeDetection.OnSwipe += SwipeDetection_Swipe;
    }
    public void onBackClicked() {
        MasksPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    void FixedUpdate() {
        if (animating) {
            Vector2 movementDelta = new Vector2(speed * Time.fixedDeltaTime, 0);

            if (animationDirection == SwipeDetection.SwipeDirection.Left) {
                foreach (Rigidbody2D rb2d in Ancors) {
                    rb2d.MovePosition(rb2d.position - movementDelta);
                }
                Vector2 p = Walls.transform.position;
                p.x -= movementDelta.x;
                Walls.transform.position = p;
                animationdelta += movementDelta.x;

                foreach (RectTransform rt in Wheels) {
                    rt.Rotate(0, 0, 180 * Time.fixedDeltaTime);
                }
            } else if (animationDirection == SwipeDetection.SwipeDirection.Right) {
                foreach (Rigidbody2D rb2d in Ancors) {
                    rb2d.MovePosition(rb2d.position + movementDelta);
                }
                Vector2 p = Walls.transform.position;
                p.x += movementDelta.x;
                Walls.transform.position = p;
                animationdelta += movementDelta.x;

                foreach (RectTransform rt in Wheels) {
                    rt.Rotate(0, 0, -180 * Time.fixedDeltaTime);
                }
            }



            if (animationdelta >= pageWidth) {
                animating = false;
            }
        } else {
            foreach (Rigidbody2D rb2d in Ancors) {
                rb2d.velocity = Vector2.zero;
            }
        }
    }


    private void SwipeDetection_Swipe(SwipeDetection.SwipeDirection dir, Vector2 delta) {
        if (!animating) {
            if (dir == SwipeDetection.SwipeDirection.Left && currPage < numPages - 1) {
                animating = true;
                animationDirection = dir;
                animationdelta = 0.0f;
                currPage++;
                if (PageChange != null)
                    PageChange(currPage);

            } else if (dir == SwipeDetection.SwipeDirection.Right && currPage > 0) {
                animating = true;
                animationDirection = dir;
                animationdelta = 0.0f;
                currPage--;
                if (PageChange != null)
                    PageChange(currPage);
            }
        }
    }


}
