using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

    [Header("Objects")]
    public Transform itemsParent;
    public Button3D openButton;
    public GameObject chestOpenParticle;
    public Button3D itemsCoverButton;
    public TextMeshPro subTitle;
    public GameObject storeBlocker;
    public Button3D rerollButton;

    [Header("Box shop")]
    public GameObject mainBuyArea;
    public GameObject popupMessage;
    public Image mainAreaBackdrop;
    public AnimationCurve popupAnimCurve;

    private Coroutine m_Routine;
    private Vector3 m_ItemsParentStartScale;
    private CustomizeButton[] m_CustomizeButtons;
    private TextMeshPro m_RerollButtonText;

    private Vector3 m_RerollButtonPosHidden;
    private Vector3 m_RerollButtonPosShown;
    private bool m_RerollButtonConfirm = false;

    private List<CustomizationUnlockData> m_UnlockData = new List<CustomizationUnlockData>();

    private void Awake() {
        m_ItemsParentStartScale = itemsParent.localScale;
        m_CustomizeButtons = itemsParent.GetComponentsInChildren<CustomizeButton>(true);
        m_RerollButtonText = rerollButton.GetComponentInChildren<TextMeshPro>();

        openButton.onButtonClick.AddListener(OpenChest);
        itemsCoverButton.onButtonClick.AddListener(UpdateState);
        rerollButton.onButtonClick.AddListener(RerollChest);

        m_RerollButtonPosShown = rerollButton.transform.localPosition;
        m_RerollButtonPosHidden = m_RerollButtonPosShown - new Vector3(0, 10, 0);

        itemsCoverButton.gameObject.SetActive(false);
        chestOpenParticle.gameObject.SetActive(false);
        rerollButton.gameObject.SetActive(false);
    }

    private void Start() {
        UpdateState();
    }
    
    public void AddChestCount(int count) {
        SaveDataManager.Instance.data.boxesToOpen += count;
        UpdateState();
        ShowPopup("Successfully purchased " + count + " lootboxes!", false);
    }

    public void ShowPurchaseFailed() {
        ShowPopup("Purchase\nfailed.", true);
    }

    public void ShowPopup(string text, bool returnToStore) {
        popupMessage.SetActive(true);
        popupMessage.GetComponentInChildren<TextMeshProUGUI>().text = text;

        StartCoroutine(PurchaseCompleteRoutine(returnToStore));
    }

    private IEnumerator PurchaseCompleteRoutine(bool returnToStore) {
        yield return null;
        mainBuyArea.SetActive(false);

        RectTransform popupTransform = popupMessage.GetComponent<RectTransform>();
        popupTransform.localScale = Vector2.zero;

        float duration = 0.5f;
        float startTime = Time.time;
        float endTime = Time.time + duration;
        while(Time.time < startTime + duration) {
            float scaler = (Time.time - startTime).Remap(0, duration, 0, 1);
            scaler = Mathf.Clamp01(scaler);
            popupTransform.localScale = Vector2.one * popupAnimCurve.Evaluate(scaler);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        Color backdropStartColor = mainAreaBackdrop.color;
        float backdropStartAlpha = backdropStartColor.a;
        startTime = Time.time;
        endTime = Time.time + duration;
        while (Time.time < startTime + duration) {
            float scaler = (Time.time - startTime).Remap(0, duration, 0, 1);
            scaler = Mathf.Clamp01(scaler);
            float curveValue = 1 - popupAnimCurve.Evaluate(scaler);
            popupTransform.localScale = Vector2.one * curveValue;
            if(!returnToStore) mainAreaBackdrop.color = new Color(backdropStartColor.r, backdropStartColor.g, backdropStartColor.b, backdropStartAlpha * curveValue);
            yield return null;
        }
        mainAreaBackdrop.color = backdropStartColor;

        mainBuyArea.SetActive(true);

        if (!returnToStore) {
            mainAreaBackdrop.canvas.gameObject.SetActive(false);
            storeBlocker.SetActive(false);
        }
    }


    public void UpdateState() {
        int boxesToOpen = SaveDataManager.Instance.data.boxesToOpen;
        subTitle.text = "Boxes to open: " + boxesToOpen;

        itemsCoverButton.gameObject.SetActive(false);
        itemsParent.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        openButton.gameObject.SetActive(true);
        rerollButton.gameObject.SetActive(false);

        if (boxesToOpen != 0) {
            ResetChest();
        }else {
            CloseChest();
        }
    }

    private void CloseChest() {
        openButton.enabled = false;
        openButton.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    private void ResetChest() {
        openButton.enabled = true;
        openButton.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OpenChest() {
        if (m_Routine != null) return;

        SaveDataManager.Instance.data.boxesToOpen--;
        SetNewLoot();

        m_Routine = StartCoroutine(OpenChestSequence());
    }

    private IEnumerator OpenChestSequence() {
        openButton.gameObject.SetActive(false);

        //Toggle particles
        chestOpenParticle.SetActive(false);
        yield return null;
        chestOpenParticle.SetActive(true);

        //Scale items
        yield return new WaitForSeconds(1);
        rerollButton.transform.localPosition = m_RerollButtonPosHidden;
        rerollButton.gameObject.SetActive(true);
        while (Vector3.Distance(itemsParent.localScale, m_ItemsParentStartScale) >= 0.01f) {
            itemsParent.localScale = Vector3.Lerp(itemsParent.localScale, m_ItemsParentStartScale, 0.1f);
            rerollButton.transform.localPosition = Vector3.Lerp(rerollButton.transform.localPosition, m_RerollButtonPosShown, 0.1f);
            yield return null;
        }

        itemsCoverButton.gameObject.SetActive(true);
        m_Routine = null;
    }

    private void RerollChest() {
        if (!m_RerollButtonConfirm) {
            m_RerollButtonText.text = "Watch ad";
            m_RerollButtonConfirm = true;
        }else {
            VideoAd.Instance.WatchAd(() => {
                m_RerollButtonText.text = "Reroll?";
                m_RerollButtonConfirm = false;

                foreach (CustomizationUnlockData lootObj in m_UnlockData) {
                    lootObj.Lock();
                }

                rerollButton.transform.localPosition = m_RerollButtonPosHidden;
                itemsParent.localScale = new Vector3(0.001f, 0.001f, 0.001f);

                SetNewLoot();
                m_Routine = StartCoroutine(OpenChestSequence());
            },
            null);
        }
    }

    private void SetNewLoot() {
        m_UnlockData.Clear();
        foreach (CustomizeButton button in m_CustomizeButtons) {
            CustomizationUnlockData lootObj = SaveDataManager.Instance.UnlockRandomItem();
            button.SetPreview(lootObj.obj);
            button.SetLocked(false);
            button.GetComponent<Button3D>().enabled = false;
            lootObj.Unlock();
            m_UnlockData.Add(lootObj);
        }
        SaveDataManager.Instance.Save();
    }
}
