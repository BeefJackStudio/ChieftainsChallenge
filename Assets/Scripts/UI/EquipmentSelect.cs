using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class is informed when a tool or mask has been selected in the equipment menu
/// it maintains the EquipmentSelection referances. Uses alpha to hide a selection when it is deselected.
/// </summary>
public class EquipmentSelect : MonoBehaviour {

    public Image SelectedTool;
    public Image SelectedMask1;
    public Image SelectedMask2;
    public Image SelectedMask3;

    public Button start;

    public string levelToLoad;


    // Use this for initialization
    void Start() {
        SelectedTool.color = new Color(1, 1, 1, 0);
        SelectedMask1.color = new Color(1, 1, 1, 0);
        SelectedMask2.color = new Color(1, 1, 1, 0);
        SelectedMask3.color = new Color(1, 1, 1, 0);
        start.interactable = false;
    }

    // called when a tool has been selected
    public void OnToolSelected(GameObject Tool) {
        Image i = Tool.GetComponent<Image>();
        SelectedTool.color = i.color;
        SelectedTool.sprite = i.sprite;
        EquipmentSelection.Tool = Tool.GetComponent<ToolTypeTag>().Type;
        checkStartState();
    }
    /// <summary>
    /// called when a mask has been selected, changes the first free space or replaces the bottom one
    /// </summary>
    /// <param name="Mask"></param>
    public void OnMaskSelected(GameObject Mask) {
        Image i = Mask.GetComponent<Image>();
        if (SelectedMask1.color.a == 0) {
            EquipmentSelection.Mask1 = Mask.GetComponent<MaskTypeTag>().Type;
            SelectedMask1.color = i.color;
            SelectedMask1.sprite = i.sprite;
        } else if (SelectedMask2.color.a == 0) {
            EquipmentSelection.Mask2 = Mask.GetComponent<MaskTypeTag>().Type;
            SelectedMask2.color = i.color;
            SelectedMask2.sprite = i.sprite;
        } else {
            EquipmentSelection.Mask3 = Mask.GetComponent<MaskTypeTag>().Type;
            SelectedMask3.color = i.color;
            SelectedMask3.sprite = i.sprite;
        }
        checkStartState();
    }

    /// <summary>
    /// called when a selected tool or mask is clicked/touched, removes the item from selection
    /// </summary>
    /// <param name="img"></param>
    public void onSelectedClicked(Image img) {
        img.color = new Color(1, 1, 1, 0);
        start.interactable = false;
    }

    /// <summary>
    /// checks that all 3 masks and a tool have been selected
    /// </summary>
    void checkStartState() {
        if (SelectedTool.color.a != 0 && SelectedMask1.color.a != 0 && SelectedMask2.color.a != 0 && SelectedMask3.color.a != 0)
            start.interactable = true;
    }

    public void onStartClicked() {
        LevelManager.LoadLevel(levelToLoad);
    }
}