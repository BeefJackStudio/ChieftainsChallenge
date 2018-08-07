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

    [Header("Box shop")]
    public GameObject mainBuyArea;
    public GameObject popupMessage;
    public Image mainAreaBackdrop;
    public AnimationCurve popupAnimCurve;

    private Coroutine m_Routine;
    private Vector3 m_ItemsParentStartScale;
    private CustomizeButton[] m_CustomizeButtons;

    private void Awake() {
        m_ItemsParentStartScale = itemsParent.localScale;
        m_CustomizeButtons = itemsParent.GetComponentsInChildren<CustomizeButton>(true);

        openButton.onButtonClick.AddListener(OpenChest);
        itemsCoverButton.onButtonClick.AddListener(UpdateState);

        itemsCoverButton.gameObject.SetActive(false);
        chestOpenParticle.gameObject.SetActive(false);
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
        foreach (CustomizeButton button in m_CustomizeButtons) {
            GameObject unlocked = SaveDataManager.Instance.UnlockRandomItem();
            button.SetPreview(unlocked);
            button.SetLocked(false);
            button.GetComponent<Button3D>().enabled = false;
        }
        SaveDataManager.Instance.Save();

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
        while (Vector3.Distance(itemsParent.localScale, m_ItemsParentStartScale) >= 0.01f) {
            itemsParent.localScale = Vector3.Lerp(itemsParent.localScale, m_ItemsParentStartScale, 0.1f);
            yield return null;
        }

        itemsCoverButton.gameObject.SetActive(true);
        m_Routine = null;
    }
}
