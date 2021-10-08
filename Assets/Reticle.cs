using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float selectAnimTime;
    [SerializeField] private float deselectAnimTime;

    [Header("Drag Settings")]
    [SerializeField] private float amplitude;
    [SerializeField] private float spaceBetween;

    [Header("Spring")]

    [SerializeField]
    private float stiffness;
    [SerializeField] private List<GameObject> points = new List<GameObject>();

    private GameObject selectedObject;

    private void Update()
    {
        if (selectedObject != null)
        {
            StopAllCoroutines();
            Selected(selectedObject);
        }
    }

    private void HandleRotation(GameObject item)
    {
        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        item.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private int HandlePoint(int type, float distance, Vector3 mousePosRelative, Point point, int i, int flipped)
    {
        type++;
        Vector3 newPos = (((mousePosRelative * type * flipped)) / (distance * amplitude));
        float lerpTime = (selectAnimTime / type) * Time.deltaTime;
        Vector3 lerpPos = Vector3.Lerp(points[i].transform.localPosition, -newPos * spaceBetween, lerpTime);
        lerpPos.z = 0;
        if (point.rigid)
            points[i].transform.localPosition = -newPos * spaceBetween;
        else
            points[i].transform.localPosition = lerpPos;

        return type;
    }
    public void Selected(GameObject selected)
    {
        this.gameObject.SetActive(true);
        this.transform.position = selected.transform.position;
        selectedObject = selected;
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 mousePosRelative = mouseWorldPos - this.transform.position;
        int fronts = 0;
        int backs = 0;
        for (int i = 0; i < points.Count; i++)
        {
            HandleRotation(points[i].gameObject);
            float distance = Vector3.Distance(this.transform.position, mouseWorldPos);
            Point point = points[i].GetComponent<Point>();
            if (point.flip)
            {
                fronts = HandlePoint(fronts, distance, mousePosRelative, point, i, 1);
            }
            else
            {
                backs = HandlePoint(backs, distance, mousePosRelative, point, i, -1);
            }
        }
    }

    public void Deselect()
    {
        selectedObject = null;
        Debug.Log("released");
        for (int i = 0; i < points.Count; i++)
        {
            StartCoroutine(LerpObject(points[i], Vector3.zero, deselectAnimTime));
        }
    }

    private IEnumerator LerpObject(GameObject item, Vector3 pos, float time)
    {
        Vector3 currentPos = item.transform.localPosition;
        float elapsed = 0f;
        float distance = Vector3.Distance(currentPos, pos);
        float ratio = 0;
        while (ratio < 1)
        {
            elapsed += Time.fixedDeltaTime;
            float offset = animCurve.Evaluate(ratio);
            float newOffset = offset - ratio;
            newOffset = newOffset / stiffness;
            offset = newOffset + ratio;
            float invertOffset = 1.0f - offset;
            item.transform.localPosition = Vector3.Lerp(currentPos, pos, ratio) * invertOffset;

            yield return null;
            ratio = (elapsed / time);
        }
    }
}
