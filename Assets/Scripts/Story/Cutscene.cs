using UnityEngine;
using System.Collections;

public class Cutscene : MonoBehaviour {

    public CutsceneTypes[] cutscenesToPlay;
    public string nextLevel;

    private Animation animator;

    private void Awake() {
        animator = GetComponent<Animation>();
    }

    private void Start() {
        foreach(CutsceneTypes cType in cutscenesToPlay) {
            animator.PlayQueued(GetAnimationClip(cType), QueueMode.CompleteOthers, PlayMode.StopSameLayer);
        }
    }

    public void OnCutsceneComplete() {
        foreach (CutsceneTypes cType in cutscenesToPlay) {
            SaveDataManager.Instance.SetCutsceneWatched(cType);
        }

        LevelManager.Instance.LoadScene(nextLevel);
    }

    private string GetAnimationClip(CutsceneTypes type) {
        switch (type) {
            case CutsceneTypes.INTRO_PART_1: return "AN_Cutscene_Intro_P01";
            case CutsceneTypes.INTRO_PART_2: return "AN_Cutscene_Intro_P02";
            case CutsceneTypes.INTRO_PART_3: return "AN_Cutscene_Intro_P03";
            case CutsceneTypes.INTRO_PART_4: return "AN_Cutscene_Intro_P04";
            case CutsceneTypes.INTRO_PART_5: return "AN_Cutscene_Intro_P05";
            case CutsceneTypes.INTRO_PART_6: return "AN_Cutscene_Intro_P06";
            default: return "";
        }
    }
}

public enum CutsceneTypes {
    INTRO_PART_1 = 0,
    INTRO_PART_2 = 1,
    INTRO_PART_3 = 2,
    INTRO_PART_4 = 3,
    INTRO_PART_5 = 4,
    INTRO_PART_6 = 5
}