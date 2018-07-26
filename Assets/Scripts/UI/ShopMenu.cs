using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopMenu : MonoBehaviour {

    [Header("Objects")]
    public Transform itemsParent;
    public Button3D openButton;
    public GameObject chestOpenParticle;
    public Button3D itemsCoverButton;
    public TextMeshPro subTitle;

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
