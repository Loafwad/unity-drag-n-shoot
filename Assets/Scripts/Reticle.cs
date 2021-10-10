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

    [Header("Spring")]
    [SerializeField] private float stiffness;

    [Header("Points")]
    [SerializeField] private List<GameObject> points = new List<GameObject>();
    private List<Vector3> pointStartPos = new List<Vector3>();
    private GameObject selectedObject;

    private void Awake()
    {
        foreach (GameObject point in points)
        {
            pointStartPos.Add(point.transform.localPosition);
        }
    }
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

    private void HandlePoint(Vector3 mousePosRelative, float distance, int i, Point point)
    {
        int flipped = 0;
        if (point.flip)
            flipped = -1;
        else
            flipped = 1;

        Vector3 startPos = pointStartPos[i];
        float magnitude = distance * (distance * amplitude);

        float pointIdentiy = (startPos.x * distance) * flipped;
        Vector3 newPos = ((mousePosRelative * pointIdentiy) / magnitude) * flipped;

        float lerpTime = (selectAnimTime / pointIdentiy) * Time.deltaTime;
        Vector3 lerpPos = Vector3.Lerp(point.transform.localPosition, newPos, lerpTime);
        lerpPos.z = 0;
        point.transform.localPosition = lerpPos;

    }

    public void Selected(GameObject selected)
    {
        this.gameObject.SetActive(true);
        this.transform.position = selected.transform.position;
        selectedObject = selected;
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 mousePosRelative = mouseWorldPos - this.transform.position;
        for (int i = 0; i < points.Count; i++)
        {
            HandleRotation(points[i].gameObject);
            float distance = Vector3.Distance(this.transform.position, mouseWorldPos);
            Point point = points[i].GetComponent<Point>();
            HandlePoint(mousePosRelative, distance, i, point);
        }
    }

    public void Deselect()
    {
        selectedObject = null;
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
