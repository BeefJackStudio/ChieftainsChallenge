using UnityEngine;
using System.Collections;

public class MinimapPanel : MonoBehaviour {

    public Camera MiniMap;
    public Camera Main;
    public RectTransform Panel;
    public RectTransform miniPlayer;
    public RectTransform minimap;
    public Canvas canvas;
    public Player Player;
    private float PanelPosX;
    private float PanelPosY;
    private float MiniPosX;
    private float MiniPosY;
    private Vector3 Minipos3;
    private float MainPosX;
    private float MainPosY;
    private Vector3 playerPos;
    private Vector3 mplayer;
    public Vector3 lastTouch;
    public Vector3 thisTouch;

    public float posXLimit;
    public float posYLimit;
    public float negXLimit;
    public float negYLimit;

    public float refocusDelay = 1.5f;
    public float speed = 4f;

    public CameraMovementBoundaries camBoundaries;

    private BallCamera ballCamera;

    void Start() {
        InputManager.inputMoved += TouchInput;
        InputManager.inputStart += TouchStart;

        ballCamera = Main.GetComponent<BallCamera>();
    }

    void TouchStart() {
        lastTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);
    }
    public void TouchInput() {
        if (minimap == null)
            return;

        //Debug.Log(minimap.rect.size.x);

        thisTouch = InputManager.InputToScreenPoint(InputManager.positionLast);

        //check if touch input is inside the minimap
        Vector2 pos = new Vector2(minimap.position.x - (minimap.rect.size.x * 0.5f * canvas.scaleFactor), minimap.position.y - (minimap.rect.size.y * 0.5f * canvas.scaleFactor));
        Rect mini = new Rect(pos, minimap.rect.size * canvas.scaleFactor);

        if (mini.Contains(thisTouch)) {
            Vector3 localTouch = Panel.gameObject.transform.parent.transform.parent.worldToLocalMatrix.MultiplyPoint(thisTouch);

            MiniPosX = MiniMap.transform.position.x;
            MiniPosY = MiniMap.transform.position.y;

            Debug.Log(localTouch);

            MainPosX = localTouch.x + MiniPosX;
            MainPosY = localTouch.y + MiniPosY;

            if (MainPosX > camBoundaries.bottomRightCorner.x ||
               MainPosX < camBoundaries.topLeftCorner.x ||
               MainPosY > camBoundaries.topLeftCorner.y ||
               MainPosY < camBoundaries.bottomRightCorner.y)
                return;

            Main.transform.position = new Vector3(MainPosX, MainPosY, Main.transform.position.z);

            //StartCoroutine(RefocusOnPlayer());
        }
    }

    void Update() {
        CameraPosition();

        PlayerPosition();
    }

    void CameraPosition() {

        MiniPosX = MiniMap.transform.position.x;
        MainPosX = Main.transform.position.x;
        MiniPosY = MiniMap.transform.position.y;
        MainPosY = Main.transform.position.y;

        PanelPosX = (MainPosX - MiniPosX);
        PanelPosY = (MainPosY - MiniPosY);

        Panel.localPosition = new Vector3(Mathf.Clamp(PanelPosX, negXLimit, posXLimit), Mathf.Clamp(PanelPosY - 30f, negYLimit, posYLimit), Panel.localPosition.z);
        //Debug.Log(PanelPos + " PanelPos");


    }

    void PlayerPosition() {
        Minipos3 = MiniMap.transform.position;
        playerPos = Player.transform.position;
        mplayer = playerPos - Minipos3;

        miniPlayer.localPosition = new Vector3(mplayer.x - 2.0f, mplayer.y - 24f, mplayer.z);

    }

    IEnumerator RefocusOnPlayer() {
        yield return new WaitForSeconds(refocusDelay);

        playerPos = Player.transform.position;

        Main.transform.position = new Vector3(Mathf.Lerp(Main.transform.position.x, playerPos.x, Time.deltaTime * speed),
                                              Mathf.Lerp(Main.transform.position.y, playerPos.y, Time.deltaTime * speed),
                                              Main.transform.position.z);
    }

}
