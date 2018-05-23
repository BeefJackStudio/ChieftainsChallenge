using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtilities {

    public static Coroutine ExecuteAfterDelay(Action action, float time, MonoBehaviour target) {
        return target.StartCoroutine(ExecuteAfterDelayRoutine(action, time));
    }

    private static IEnumerator ExecuteAfterDelayRoutine(Action action, float time) {
        yield return new WaitForSeconds(time);
        action();
    }
}
