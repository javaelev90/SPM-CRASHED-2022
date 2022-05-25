using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveViewer : MonoBehaviour
{
    [SerializeField] private RectTransform displayObjectivePosition;
    [SerializeField] private RectTransform hideObjectivePosition;
    [SerializeField] private RectTransform objectivePanel;
    [SerializeField] private float smoothTime;
    [SerializeField] private TMP_Text upgradedPartsText;
    [SerializeField] private TMP_Text totalNumberText;

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

    public void UpdateUpgradedShipParts(int upgraded)
    {
        upgradedPartsText.text = upgraded.ToString();
    }

    public void InitializeShipPartsAmount(int completed, int total)
    {
        upgradedPartsText.text = completed.ToString();
        totalNumberText.text = total.ToString();
    }
     
}
