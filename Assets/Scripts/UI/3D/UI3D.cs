using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI3D : MonoBehaviour {

    public LayerMask layers;
    public AnimationCurve cameraMoveCurve;

    private Camera m_Camera;
    private Coroutine m_CameraMoveRoutine;

    private void Start() {
        m_Camera = Camera.main;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, layers)) {
                UI3DElement element = hit.transform.GetComponent<UI3DElement>();
                if (element == null) return;

                element.OnInteract();
            }
        }
    }

    public void MoveCameraTo(Transform target) {
        if (m_CameraMoveRoutine != null) StopCoroutine(m_CameraMoveRoutine);
        m_CameraMoveRoutine = StartCoroutine(MoveCameraRoutine(target));
    }

    private IEnumerator MoveCameraRoutine(Transform target) {
        Vector3 startPosition = m_Camera.transform.position;
        Vector3 endPosition = target.position;
        Quaternion startRotation = m_Camera.transform.rotation;
        Quaternion endRotation = target.rotation;
        float startTime = Time.time;

        float animationDuration = 1f;

        float normalizedTime = 0;
        while(normalizedTime < 1) {
            normalizedTime = MathUtilities.GetNormalizedTime(startTime, animationDuration, Time.time);
            float animationCurveValue = cameraMoveCurve.Evaluate(normalizedTime);
            m_Camera.transform.position = Vector3.Lerp(startPosition, endPosition, animationCurveValue);
            m_Camera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, animationCurveValue);
            yield return null;
        }

        m_CameraMoveRoutine = null;
    }
}
