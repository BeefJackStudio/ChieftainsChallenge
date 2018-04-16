using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// This script is designed to be placed on each contnent on the world map, when click it shows the level select pop up all levels and selects the prefab based on its availiblity
/// </summary>
public class LevelSelectContinent : MonoBehaviour {

    public GameObject LockedButtonPrefab;
    public GameObject UnlockedButtonPrefab;
    public GameObject CompleteButtonPrefab;

    public GameObject LevelSelectPanel;
    public GameObject ScrollContent;
    public GameObject Title;
    public GameObject Mask;
    public GameObject EquipmentPanel;

    private int buttonHeight = 150;
    private int buttonWidth = 120;
    // Use this for initialization
    void Start() {
        EventSystem.current.pixelDragThreshold = Mathf.CeilToInt(Screen.height * 0.07f);
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// called when a continent is clicked or touched
    /// </summary>
    /*public void onContinentSelected() {

        if (!LevelSelectPanel.activeSelf) {
            LevelSelectPanel.SetActive(true);

            foreach (Transform child in ScrollContent.transform) {
                //remove any buttons from the container incase another continent was selected previously
                GameObject.Destroy(child.gameObject);
            }

            Title.GetComponent<Text>().text = gameObject.name;

            RectTransform rectt = ScrollContent.transform as RectTransform;
            rectt.sizeDelta = new Vector2(0, ((Mathf.CeilToInt(Levels.Count / 4.0f)) * ScrollContent.GetComponent<GridLayoutGroup>().cellSize.y));



            for (int i = 0; i < Levels.Count; i++) {
                Level currLevel = SaveData.currentSave.getLevel(Type, i);
                LevelState currState = Levels[i].DefaultState;
                if (currLevel != null) {
                    currState = currLevel.state;
                } else {
                    if (i == 0 && currState == LevelState.Locked) {
                        currState = LevelState.Unlocked; // unlock first level for given continent
                    }
                    SaveData.currentSave.addLevel(Type, currState);
                }


                GameObject b;


                if (currState == LevelState.Unlocked)
                    b = Instantiate(UnlockedButtonPrefab) as GameObject;
                else if (currState == LevelState.Locked)
                    b = Instantiate(LockedButtonPrefab) as GameObject;
                else //must be atleast one star
                {
                    b = Instantiate(CompleteButtonPrefab) as GameObject;
                    b.GetComponent<SetStars>().updateStars(currState);
                }

                b.transform.SetParent(ScrollContent.transform, true);
                b.transform.localScale = Vector3.one;

                b.GetComponentInChildren<Text>().text = (i + 1).ToString();
                b.name = (i + 1).ToString();

                b.GetComponentInChildren<Button>().onClick.AddListener(delegate { onLevelSelected(b.name); });
                b.GetComponentInChildren<Button>().onClick.AddListener(delegate { menuSFX.onButtonPressed(); });

            }

        }

    }

    void onLevelSelected(string index) {
        Debug.Log("Button Pressed: " + index);


        SaveData.CurrentContinent = Type;
        SaveData.CurrentLevel = int.Parse(index) - 1;
        SaveData.CurrentLevelName = Levels[SaveData.CurrentLevel].LevelName;
        EquipmentPanel.GetComponent<EquipmentSelect>().levelToLoad = Levels[SaveData.CurrentLevel].LevelName;//levelToLoad
        EquipmentPanel.SetActive(true);
    }*/


}