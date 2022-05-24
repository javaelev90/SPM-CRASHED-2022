using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveViewer : MonoBehaviour
{
    [SerializeField] private RectTransform displayObjectivePosition;
    [SerializeField] private RectTransform hideObjectivePosition;
    [SerializeField] private RectTransform objectivePanel;
    [SerializeField] private float smoothTime;

    public bool IsDisplayingPanel { get; set; }

    void Update()
    {
        if (IsDisplayingPanel == true)
        {
            objectivePanel.anchoredPosition = Vector2.Lerp(objectivePanel.anchoredPosition, displayObjectivePosition.anchoredPosition, smoothTime);
            if (Vector2.Distance(objectivePanel.anchoredPosition, displayObjectivePosition.anchoredPosition) < 0.01f) { this.enabled = false; }
        }

        if (IsDisplayingPanel == false)
        {
            objectivePanel.anchoredPosition = Vector2.Lerp(objectivePanel.anchoredPosition, hideObjectivePosition.anchoredPosition, smoothTime);
            if (Vector2.Distance(objectivePanel.anchoredPosition, hideObjectivePosition.anchoredPosition) < 0.01f) { this.enabled = false; }
        }
    }

}
