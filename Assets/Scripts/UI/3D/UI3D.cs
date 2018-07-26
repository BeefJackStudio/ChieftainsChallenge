 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI3D : MonoBehaviourSingleton<UI3D> {

    public LayerMask layers;
    public AnimationCurve cameraMoveCurve;

    private Camera m_Camera;
    private Coroutine m_CameraMoveRoutine;
    private string m_CurrentLocation = "";

    public string GetCurrentLocation() {
        return m_CurrentLocation;
    }

    private void Start() {
        m_Camera = Camera.main;
    }

    private void Update() {
        if (VideoAdMenu.Instance.gameObject.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000, layers)) {
                UI3DElement element = hit.transform.GetComponent(typeof(UI3DElement)) as UI3DElement;
                if (element == null) return;

                element.OnInteract(hit);
            }
        }
    }

    public bool RaycastMouseOnPlane(Vector3 normal, Vector3 position, out Vector3 hitPosition) {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(normal, position);
        float hitRange;

        if (plane.Raycast(ray, out hitRange)) {
            hitPosition = ray.GetPoint(hitRange);
            return true;
        }else {
            hitPosition = Vector3.zero;
            return false;
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
        m_CurrentLocation = target.name;

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
