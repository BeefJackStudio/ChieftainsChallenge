using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwingInputUI : MonoBehaviour {

    public GameObject PAPanel;
    public RectTransform BackPosImage;
    public RectTransform StartLineImage;
    public RectTransform EndLineImage;
    public RectTransform VLine1;
    public RectTransform VLine2;
    public RectTransform HLine;

    public Text Angle;
    public RectTransform AngleLine;
    public Image BaseLine;
    public Text Power;

    public AnimationCurve TimeBones;
    public Gradient colourTimeBones;

    private float pointerLength;
    private int shotCount;

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        NotificationsManager.OnGameStateChange += NotificationsManager_OnGameStateChange;
        NotificationsManager.OnGameStateStay += NotificationsManager_OnGameStateStay;
        NotificationsManager.OnInputStateChange += NotificationsManager_OnInputStateChange;
        NotificationsManager.OnSwingUIColourUpdate += NotificationsManager_OnSwingUIColourUpdate;
        NotificationsManager.OnSwingBackAngleChange += NotificationsManager_OnSwingBackAngleChange;
        NotificationsManager.OnSwingAngleData += NotificationsManager_OnPrintSwingAngleData;
        NotificationsManager.UpdateInGameUI += NotificationsManager_UpdateInGameUI;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        NotificationsManager.OnGameStateChange -= NotificationsManager_OnGameStateChange;
        NotificationsManager.OnGameStateStay -= NotificationsManager_OnGameStateStay;
        NotificationsManager.OnInputStateChange -= NotificationsManager_OnInputStateChange;
        NotificationsManager.OnSwingUIColourUpdate -= NotificationsManager_OnSwingUIColourUpdate;
        NotificationsManager.OnSwingBackAngleChange -= NotificationsManager_OnSwingBackAngleChange;
        NotificationsManager.OnSwingAngleData -= NotificationsManager_OnPrintSwingAngleData;
        NotificationsManager.UpdateInGameUI -= NotificationsManager_UpdateInGameUI;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnSwingUIColourUpdate(float shotTime) {
        UpdateUIColour(shotTime);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_UpdateInGameUI(int distance, int currentScore, int thisShotCount) {
        shotCount = thisShotCount;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnSwingBackAngleChange(float angle, float len, bool startLine) {
        if (startLine)
            UpdateStartLineRotationAndSize(angle, len);
        else
            UpdateEndLineRotationAndSize(angle);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnPrintSwingAngleData(float angle, float power) {
        PrintAngleUIData(angle, power);
        RemoveUIObjects();
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateChange(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.FollowBall:
                if (shotCount != -1) {
                    PAPanel.SetActive(true);
                    ResetUIColour();
                }
                break;
            case GameManager.GameState.FreeView:
                PAPanel.SetActive(false);
                PAPanel.transform.localPosition = new Vector3(0, 0, 0);
                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnGameStateStay(GameManager.GameState currentState) {
        switch (currentState) {
            case GameManager.GameState.FollowBall:
                if (shotCount != -1) {
                    StartCoroutine(LerpPAPanel());
                }
                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_OnInputStateChange(GameManager.InputState currentState, Vector2 backPos) {
        switch (currentState) {
            case GameManager.InputState.SwingForword:
                SetEndLinePositions(backPos);
                break;
            case GameManager.InputState.SwingBack:
                SetStartLinePositions(backPos);
                break;
            case GameManager.InputState.PlayerSelected:
                RemoveUIObjects();
                break;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void UpdateUIColour(float shotTime) {
        float colourIndex = (1.0f / 2.5f) * shotTime;
        Color col = colourTimeBones.Evaluate(colourIndex);
        Angle.GetComponent<Outline>().effectColor = col;
        Power.GetComponent<Outline>().effectColor = col;
        AngleLine.GetComponent<Image>().color = col;
        BaseLine.color = col;
        BackPosImage.GetComponent<Image>().color = col;
        StartLineImage.GetComponent<Image>().color = col;
        EndLineImage.GetComponent<Image>().color = col;
        VLine1.GetComponent<Image>().color = col;
        VLine2.GetComponent<Image>().color = col;
        HLine.GetComponent<Image>().color = col;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void ResetUIColour() {
        BackPosImage.GetComponent<Image>().color = Color.red;
        StartLineImage.GetComponent<Image>().color = Color.red;
        EndLineImage.GetComponent<Image>().color = Color.red;
        VLine1.GetComponent<Image>().color = Color.red;
        VLine2.GetComponent<Image>().color = Color.red;
        HLine.GetComponent<Image>().color = Color.red;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void SetStartLinePositions(Vector2 ball2d) {
        StartLineImage.gameObject.SetActive(true);
        StartLineImage.position = ball2d;
        Vector2 vpos = VLine1.position;
        vpos.x = StartLineImage.position.x;
        VLine1.position = vpos;
        vpos = VLine2.position;
        vpos.x = StartLineImage.position.x;
        VLine2.position = vpos;
        VLine1.gameObject.SetActive(true);
        VLine2.gameObject.SetActive(true);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void SetEndLinePositions(Vector2 backPos) {
        BackPosImage.gameObject.SetActive(true);
        BackPosImage.position = backPos;
        EndLineImage.gameObject.SetActive(true);
        EndLineImage.position = backPos;
        Vector2 size = EndLineImage.sizeDelta;
        size.x = InputManager.InputToScreenPoint(new Vector2(10, 1)).x;
        EndLineImage.sizeDelta = size;
        Vector2 hpos = HLine.position;
        hpos.y = BackPosImage.position.y;
        HLine.position = hpos;
        HLine.gameObject.SetActive(true);
        pointerLength = 10;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void UpdateStartLineRotationAndSize(float angle, float len) {
        Vector3 rot = StartLineImage.eulerAngles;
        rot.z = (angle + 180);
        StartLineImage.eulerAngles = rot;

        Vector2 size = StartLineImage.sizeDelta;
        size.x = len;
        StartLineImage.sizeDelta = size;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void UpdateEndLineRotationAndSize(float angle) {
        Vector3 rot = EndLineImage.eulerAngles;
        rot.z = angle;
        EndLineImage.eulerAngles = rot;

        pointerLength = Mathf.Lerp(pointerLength, 500 * InputManager.screenScale.x, Time.deltaTime * 3);
        Vector2 size = EndLineImage.sizeDelta;
        size.x = InputManager.InputToScreenPoint(new Vector2(pointerLength, 1)).x;
        EndLineImage.sizeDelta = size;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void PrintAngleUIData(float angle, float power) {
        Vector3 rot = Angle.GetComponent<RectTransform>().eulerAngles;
        rot.z = angle;
        Angle.GetComponent<RectTransform>().eulerAngles = rot;
        AngleLine.eulerAngles = rot;

        Angle.text = angle.ToString("F2") + "°";
        Power.text = (power * 100).ToString("F2") + "%";
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    void RemoveUIObjects() {
        BackPosImage.gameObject.SetActive(false);
        StartLineImage.gameObject.SetActive(false);
        EndLineImage.gameObject.SetActive(false);
        VLine1.gameObject.SetActive(false);
        VLine2.gameObject.SetActive(false);
        HLine.gameObject.SetActive(false);

    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------

    IEnumerator LerpPAPanel() {
        if (PAPanel.transform.position.y > -250f) {
            Vector3 pos = PAPanel.transform.localPosition;
            pos.y = Mathf.Lerp(pos.y, -250, Time.deltaTime * 3);
            PAPanel.transform.localPosition = pos;

            yield return null;
        }
    }
}
